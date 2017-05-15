import {Component, Input, EventEmitter, Output, OnInit, OnChanges} from '@angular/core';

import {Origin} from '../origin';
import {StatRequirement} from '../shared';
import {Job} from './job.model';
import {JobService} from './job.service';
import {getRandomInt} from '../shared/random';

@Component({
    selector: 'job-selector',
    templateUrl: './job-selector.component.html',
    styleUrls: ['./job-selector.component.scss'],
})
export class JobSelectorComponent implements OnInit, OnChanges {
    @Input('cou') cou: string;
    @Input('cha') cha: string;
    @Input('int') int: string;
    @Input('ad') ad: string;
    @Input('fo') fo: string;

    @Output() jobChange: EventEmitter<Job> = new EventEmitter<Job>();
    @Input() selectedJob: Job;
    @Input() selectedOrigin: Origin;
    public detail: {[originId: number]: boolean} = {};

    public jobs: Job[] = [];
    public stats: any;

    public invalidStats: any[] = [];
    public viewNotAvailable = false;

    constructor(private _jobService: JobService) {
        this.stats = this;
    }

    isVisible(job: Job) {
        if (this.selectedOrigin) {
            if (job.originsWhitelist && job.originsWhitelist.length > 0) {
                let found = false;
                for (let i = 0; i < job.originsWhitelist.length; i++) {
                    if (job.originsWhitelist[i].id === this.selectedOrigin.id) {
                        found = true;
                    }
                }
                if (!found) {
                    return false;
                }
            }

            if (job.originsBlacklist && job.originsBlacklist.length > 0) {
                for (let i = 0; i < job.originsBlacklist.length; i++) {
                    if (job.originsBlacklist[i].id === this.selectedOrigin.id) {
                        return false;
                    }
                }
            }

        }
        if (!this.viewNotAvailable && !this.isAvailable(job)) {
            return false;
        }
        return true;
    }

    updateInvalidStats(job: Job) {
        if (!this.isVisible(job)) {
            return [];
        }

        let invalids = [];
        if (this.selectedOrigin) {
            if (job.isMagic) {
                if (this.selectedOrigin.restrictsTokens) {
                    for (let i = 0; i < this.selectedOrigin.restrictsTokens.length; i++) {
                        if (this.selectedOrigin.restrictsTokens[i] === 'NO_MAGIC') {
                            invalids.push({stat: 'MAGIC'});
                        }
                    }
                }
            }
        }

        if (job.requirements) {
            for (let i = 0; i < job.requirements.length; i++) {
                let req: StatRequirement;
                req = job.requirements[i];
                let statName = req.stat.toLowerCase();
                let statValue = this[statName];
                if (statValue) {
                    if (req.min && statValue < req.min) {
                        invalids.push({stat: statName, min: req.min});
                    }
                    if (req.max && statValue > req.max) {
                        invalids.push({stat: statName, max: req.min});
                    }

                }
            }
        }
        return invalids;
    }

    isAvailable(job: Job) {
        if (this.selectedJob) {
            return (this.selectedJob.id === job.id);
        }
        return !(this.invalidStats[job.id] && this.invalidStats[job.id].length);
    }

    selectJob(job: Job) {
        if (job == null) {
            this.jobChange.emit(job);
            return false;
        }
        if (!this.isVisible(job) || !this.isAvailable(job)) {
            return false;
        }
        this.selectedJob = job;
        this.jobChange.emit(job);
        return false;
    }

    updateInvalids() {
        for (let i = 0; i < this.jobs.length; i++) {
            let job = this.jobs[i];
            this.invalidStats[job.id] = this.updateInvalidStats(job);
        }
    }

    getJobs() {
        this._jobService.getJobList().subscribe(
            jobs => {
                this.jobs = jobs;
                this.updateInvalids();
            },
            err => {
                console.log(err);
            }
        );
    }

    toggleViewAll(): void {
        this.viewNotAvailable = !this.viewNotAvailable;
    }

    randomSelect(): void {
        let count = 1;
        for (let i = 0; i < this.jobs.length; i++) {
            let job = this.jobs[i];
            if (this.isVisible(job) && this.isAvailable(job)) {
                count++;
            }
        }
        let rnd = getRandomInt(1, count);
        count = 0;
        for (let i = 0; i < this.jobs.length; i++) {
            let job = this.jobs[i];
            if (this.isVisible(job) && this.isAvailable(job)) {
                count++;
                if (count === rnd) {
                    this.selectJob(this.jobs[i]);
                    return;
                }
            }
        }
        this.selectJob(null);
    }

    toggleDetail(origin: Origin) {
        this.detail[origin.id] = !this.detail[origin.id];
    }

    displayDetail(origin: Origin) {
        return this.detail[origin.id];
    }

    ngOnChanges() {
        this.updateInvalids();
    }

    ngOnInit() {
        this.getJobs();
    }
}
