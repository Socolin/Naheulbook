
import {forkJoin, ReplaySubject, Observable} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {Job} from './job.model';
import {Skill, SkillService} from '../skill';

@Injectable()
export class JobService {
    private jobs: ReplaySubject<Job[]>;

    constructor(private httpClient: HttpClient
        , private _skillService: SkillService) {
    }

    getJobList(): Observable<Job[]> {
        if (!this.jobs) {
            this.jobs = new ReplaySubject<Job[]>(1);

            forkJoin([
                this._skillService.getSkillsById(),
                this.httpClient.get('/api/v2/jobs')
            ]).subscribe(
                ([skillsById, jobsDatas]: [{[skillId: number]: Skill}, Job[]]) => {
                    let jobs: Job[] = [];
                    let jobIds: {[jobId: number]: Job} = {};
                    for (let jobData of jobsDatas) {
                        let job = Job.fromJson(jobData, skillsById);
                        Object.freeze(job);
                        jobIds[job.id] = job;
                        jobs.push(job);
                    }

                    for (let i = 0; i < jobs.length; i++) {
                        let job = jobs[i];
                        if (job.parentJobId) {
                            let jobCopy: Job = Job.fromJson(JSON.parse(JSON.stringify(jobIds[job.parentJobId])), skillsById);
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

    getJobsById(): Observable<{[jobId: number]: Job}> {
        return this.getJobList().pipe(map((jobs: Job[]) => {
            let jobsById = {};
            jobs.map(j => jobsById[j.id] = j);
            return jobsById;
        }));
    }

    getJobsNamesById(): Observable<{[jobId: number]: string}> {
        return this.getJobList().pipe(map((jobs: Job[]) => {
            let jobNamesById = {};
            jobs.map(j => jobNamesById[j.id] = j.name);
            return jobNamesById;
        }));
    }
}
