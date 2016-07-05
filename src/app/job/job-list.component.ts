import {Component, OnInit} from '@angular/core';
import {NotificationsService} from '../notifications';

import {JobComponent} from "./job.component";
import {Job} from "./job.model";
import {JobService} from "./job.service";

@Component({
    moduleId: module.id,
    selector: 'job-list',
    templateUrl: 'job-list.component.html',
    directives: [JobComponent]
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
