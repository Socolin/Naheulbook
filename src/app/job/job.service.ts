import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';

import {Job} from './job.model';
import {Skill, SkillService} from '../skill';

@Injectable()
export class JobService {
    private jobs: ReplaySubject<Job[]>;

    constructor(private _http: Http
        , private _skillService: SkillService) {
    }

    getJobList(): Observable<Job[]> {
        if (!this.jobs) {
            this.jobs = new ReplaySubject<Job[]>(1);

            Observable.forkJoin(
                this._skillService.getSkillsById(),
                this._http.get('/api/job/list').map(res => res.json())
            ).subscribe(
                res => {
                    let skillsById: {[skillId: number]: Skill} = res[0];
                    let jobs: Job[] = res[1];
                    let jobIds: {[jobId: number]: Job} = {};
                    for (let i = 0; i < jobs.length; i++) {
                        let job = jobs[i];
                        for (let s = 0; s < job.skills.length; s++) {
                            let skill = job.skills[s];
                            job.skills[s] = skillsById[skill.id];
                        }
                        for (let s = 0; s < job.availableSkills.length; s++) {
                            let skill = job.availableSkills[s];
                            job.availableSkills[s] = skillsById[skill.id];
                        }
                        jobIds[job.id] = job;
                        Object.freeze(job);
                    }

                    for (let i = 0; i < jobs.length; i++) {
                        let job = jobs[i];
                        if (job.parentJobId) {
                            let jobCopy: Job = JSON.parse(JSON.stringify(jobIds[job.parentJobId]));
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
}
