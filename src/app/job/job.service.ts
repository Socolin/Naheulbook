import {forkJoin, ReplaySubject, Observable} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {Job, JobDictionary} from './job.model';
import {SkillService} from '../skill';
import {NamesByNumericId} from '../shared/shared,model';
import {JobResponse} from '../api/responses';

@Injectable()
export class JobService {
    private jobs: ReplaySubject<Job[]>;

    constructor(
        private readonly httpClient: HttpClient,
        private readonly skillService: SkillService,
    ) {
    }

    getJobList(): Observable<Job[]> {
        if (!this.jobs) {
            this.jobs = new ReplaySubject<Job[]>(1);

            forkJoin([
                this.skillService.getSkillsById(),
                this.httpClient.get<JobResponse[]>('/api/v2/jobs')
            ]).subscribe(
                ([skillsById, responses]) => {
                    const jobs = responses.map(response => Job.fromResponse(response, skillsById));
                    const jobsByIds = jobs.reduce((dictionary, job) => {
                        dictionary[job.id] = job;
                        return dictionary;
                    }, {});
                    jobs.forEach(job => Object.freeze(job));

                    // FIXME: Find a better way to handle all the job blacklist/whitelist system.
                    // It's really useful for only one job and it could be done using a less hacky way
                    for (let i = 0; i < jobs.length; i++) {
                        let job = jobs[i];
                        if (job.parentJobId) {
                            let jobCopy: Job = Job.fromResponse(JSON.parse(JSON.stringify(jobsByIds[job.parentJobId])), skillsById);
                            jobCopy.originsBlacklist = [];
                            jobCopy.originsWhitelist = [];
                            for (let field in job) {
                                if (job.hasOwnProperty(field)) {
                                    if (job[field] != null && (!Array.isArray(job[field]) || job[field].length > 0)) {
                                        jobCopy[field] = job[field];
                                    }
                                }
                            }
                            Object.freeze(jobCopy);
                            jobs[i] = jobCopy;
                        }
                    }

                    Object.freeze(jobs);
                    this.jobs.next(jobs);
                    this.jobs.complete();
                },
                error => {
                    this.jobs.error(error);
                }
            );
        }
        return this.jobs;
    }

    getJobsById(): Observable<JobDictionary> {
        return this.getJobList().pipe(map((jobs) => {
            return jobs.reduce((dictionary: JobDictionary, job) => {
                dictionary[job.id] = job;
                return dictionary;
            }, {});
        }));
    }

    getJobsNamesById(): Observable<NamesByNumericId> {
        return this.getJobList().pipe(map((jobs: Job[]) => {
            return jobs.reduce((dictionary: NamesByNumericId, job) => {
                dictionary[job.id] = job.name;
                return dictionary;
            }, {});
        }));
    }
}
