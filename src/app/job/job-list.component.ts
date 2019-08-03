import {Component, OnInit} from '@angular/core';
import {NotificationsService} from '../notifications';

import {Job} from './job.model';
import {JobService} from './job.service';

@Component({
    selector: 'job-list',
    templateUrl: './job-list.component.html'
})
export class JobListComponent implements OnInit {
    public jobs: Job[];

    constructor(private _jobService: JobService) {
    }

    getJobs() {
        this._jobService.getJobList().subscribe(
            jobs => {
                this.jobs = jobs;
            }
        );
    }

    ngOnInit() {
        this.getJobs();
    }
}
