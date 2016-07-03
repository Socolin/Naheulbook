import {Component} from '@angular/core';
import {NotificationsService} from '../notifications';

import {JobComponent} from "./job.component";
import {Job} from "./job.model";
import {JobService} from "./job.service";

@Component({
    selector: 'job-list',
    templateUrl: 'app/job/job-list.component.html',
    styleUrls: ['/styles/job-list.css'],
    directives: [JobComponent]
})
export class JobListComponent {
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
