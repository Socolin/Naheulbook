import {Injectable, EventEmitter} from '@angular/core';
import {Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';

import {Job} from "./job.model";

@Injectable()
export class JobService {
    private jobs: ReplaySubject<Job[]>;

    constructor(private _http: Http) {
    }

    getJobById(jobId: number): Observable<Job> {
        let observable: EventEmitter<Job> = new EventEmitter<Job>();

        this.getJobList().subscribe(jobs => {
            for (let i = 0; i < jobs.length; i++) {
                let job = jobs[i];
                if (job.id === jobId) {
                    observable.emit(job);
                    return;
                }
            }
            observable.error('Invalid job id: ' + jobId);
        });
        return observable;
    }

    getJobList(): Observable<Job[]> {
        if (!this.jobs || this.jobs.isUnsubscribed) {
            this.jobs = new ReplaySubject<Job[]>(1);

            this._http.get('/api/job/list')
                .map(res => res.json())
                .subscribe(
                    jobs => {
                        this.jobs.next(jobs);
                    },
                    error => {
                        this.jobs.error(error);
                    }
                );
        }
        return this.jobs;
    }
}
