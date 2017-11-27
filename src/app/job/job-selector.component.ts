import {Component, Input, EventEmitter, Output, OnInit, OnChanges} from '@angular/core';

import {generateAllStatsPair, getRandomInt} from '../shared';

import {Origin} from '../origin';

import {Job} from './job.model';
import {JobService} from './job.service';

@Component({
    selector: 'job-selector',
    templateUrl: './job-selector.component.html',
    styleUrls: ['./job-selector.component.scss'],
})
export class JobSelectorComponent implements OnInit, OnChanges {
    @Input('cou') cou: number;
    @Input('cha') cha: number;
    @Input('int') int: number;
    @Input('ad') ad: number;
    @Input('fo') fo: number;

    @Output() jobChange: EventEmitter<Job> = new EventEmitter<Job>();
    @Output() swapStats: EventEmitter<string[]> = new EventEmitter<string[]>();
    @Input() allowSwapStats = true;
    @Input() displayNoJobOption = true;
    @Input() selectedJobs?: Job[];
    @Input() selectedOrigin: Origin;

    public stats: { [statName: string]: number } = {};
    public allJobs: Job[] = [];
    public jobs: Job[] = [];
    public jobsStates: { [jobId: number]: { state: string, changes?: any[] } };
    public swapList: string[][];

    static isJobValid(job: Job, stats: { [statName: string]: number }): boolean {
        for (let req of job.requirements) {
            let statName = req.stat.toLowerCase();
            let statValue = stats[statName];
            if (statValue) {
                if (req.min && statValue < req.min) {
                    return false;
                }
                if (req.max && statValue > req.max) {
                    return false;
                }
            }
        }
        return true;
    }

    constructor(private _jobService: JobService) {
        this.swapList = generateAllStatsPair();
    }

    isVisible(job: Job) {
        if (this.selectedJobs && this.selectedJobs.find(j => j.id === job.id)) {
            return false;
        }
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
        return true;
    }

    updateJobStates() {
        this.updateStats();
        let jobsStates = {};
        for (let job of this.jobs) {
            if (JobSelectorComponent.isJobValid(job, this.stats)) {
                jobsStates[job.id] = {state: 'ok'};
            }
            else {
                if (!this.allowSwapStats) {
                    jobsStates[job.id] = {state: 'ko'};
                    continue;
                }
                let validSwap: string[][] = [];
                for (let swap of this.swapList) {
                    let testStats = Object.assign({}, this.stats);
                    let tmp = testStats[swap[0]];
                    testStats[swap[0]] = testStats[swap[1]];
                    testStats[swap[1]] = tmp;
                    if (JobSelectorComponent.isJobValid(job, testStats)) {
                        validSwap.push(swap);
                    }
                }
                if (validSwap.length) {
                    jobsStates[job.id] = {state: 'swap', changes: validSwap};
                } else {
                    jobsStates[job.id] = {state: 'ko'};
                }
            }
        }
        this.jobsStates = jobsStates;
    }

    isAvailable(job: Job) {
        return this.jobsStates[job.id].state === 'ok';
    }

    selectJob(job: Job | undefined) {
        if (!job) {
            this.jobChange.emit(job);
            return false;
        }
        this.jobChange.emit(job);
        return false;
    }

    updateJobs() {
        let jobs: Job[] = [];
        for (let job of this.allJobs) {
            if (this.isVisible(job)) {
                jobs.push(job);
            }
        }
        this.jobs = jobs;
    }

    getJobs() {
        this._jobService.getJobList().subscribe(
            jobs => {
                this.allJobs = jobs;
                this.updateJobs();
                this.updateJobStates();
            },
            err => {
                console.log(err);
            }
        );
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
        this.selectJob(undefined);
    }

    private updateStats() {
        this.stats = {
            cou: this.cou,
            cha: this.cha,
            fo: this.fo,
            ad: this.ad,
            int: this.int
        };
    }

    doSwapStats(change: string[]) {
        this.swapStats.emit(change);
    }

    ngOnChanges() {
        this.updateJobs();
        this.updateJobStates();
    }

    ngOnInit() {
        this.getJobs();
    }
}
