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
