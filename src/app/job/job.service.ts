import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';

import {Job} from "./job.model";

@Injectable()
export class JobService {
    private jobs: ReplaySubject<Job[]>;

    constructor(private _http: Http) {
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
