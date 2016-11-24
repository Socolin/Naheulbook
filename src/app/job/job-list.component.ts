import {Component, OnInit} from '@angular/core';
import {NotificationsService} from '../notifications';

import {Job} from './job.model';
import {JobService} from './job.service';

@Component({
    selector: 'job-list',
    templateUrl: 'job-list.component.html'
})
export class JobListComponent implements OnInit {
    public jobs: Job[];

    constructor(private _jobService: JobService
        , private _notification: NotificationsService) {
    }

    getJobs() {
        this._jobService.getJobList().subscribe(
            jobs => {
                this.jobs = jobs;
            },
            err => {
                this._notification.error('Erreur', 'Erreur serveur');
                console.log(err);
            }
        );
    }

    ngOnInit() {
        this.getJobs();
    }
}
