import {Component, OnInit} from '@angular/core';

import {Job} from './job.model';
import {JobService} from './job.service';

@Component({
    selector: 'job-list',
    templateUrl: './job-list.component.html',
    styleUrls: ['./job-list.component.scss'],
})
export class JobListComponent implements OnInit {
    public jobs: Job[];

    constructor(
        private readonly jobService: JobService,
    ) {
    }

    getJobs() {
        this.jobService.getJobList().subscribe(
            jobs => {
                this.jobs = jobs;
            }
        );
    }

    ngOnInit() {
        this.getJobs();
    }
}
