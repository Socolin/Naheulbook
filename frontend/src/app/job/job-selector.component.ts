import {Component, EventEmitter, Input, OnChanges, OnInit, Output} from '@angular/core';

import {generateAllStatsPair, getRandomInt} from '../shared';

import {Origin} from '../origin';

import {Job} from './job.model';
import {JobService} from './job.service';

type JobAvailability = {
    title: string,
    state: 'ok' | 'ko' | 'swap',
    icon: string,
    jobs: Job[]
};

@Component({
    selector: 'job-selector',
    templateUrl: './job-selector.component.html',
    styleUrls: ['./job-selector.component.scss'],
})
export class JobSelectorComponent implements OnInit, OnChanges {
    @Input() cou: number;
    @Input() cha: number;
    @Input() int: number;
    @Input() ad: number;
    @Input() fo: number;

    @Output() jobChange: EventEmitter<Job> = new EventEmitter<Job>();
    @Output() swapStats: EventEmitter<string[]> = new EventEmitter<string[]>();
    @Input() allowSwapStats = true;
    @Input() displayNoJobOption = true;
    @Input() selectedJobs?: readonly Job[];
    @Input() selectedOrigin: Origin;

    public stats: { [statName: string]: number } = {};
    public jobs: Job[] = [];
    public jobsStates: { [jobId: string]: { changes?: any[] } };
    public swapList: string[][];
    public availabilityOk: JobAvailability;
    public availabilities: JobAvailability[] = [];

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

    constructor(
        private readonly jobService: JobService,
    ) {
        this.swapList = generateAllStatsPair();
    }

    updateJobStates() {
        this.updateStats();
        let jobsStates = {};
        const {availabilityOk, availabilitySwap, availabilityKo} = this.createAvailabilities();

        for (let job of this.jobs) {
            if (this.selectedJobs && this.selectedJobs.findIndex(j => j.id === job.id) !== -1) {
                continue;
            }

            if (JobSelectorComponent.isJobValid(job, this.stats)) {
                availabilityOk.jobs.push(job);
            } else {
                if (!this.allowSwapStats) {
                    availabilityKo.jobs.push(job);
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
                    availabilitySwap.jobs.push(job);
                    jobsStates[job.id] = {changes: validSwap};
                } else {
                    availabilityKo.jobs.push(job);
                }
            }
        }
        this.jobsStates = jobsStates;
        this.availabilities = [
            availabilityOk,
            availabilitySwap,
            availabilityKo
        ];
        this.availabilityOk = availabilityOk;
    }

    private createAvailabilities() {
        const availabilityOk = {
            title: 'Métiers disponibles',
            state: 'ok',
            icon: 'check',
            jobs: []
        } as JobAvailability;
        const availabilitySwap = {
            title: 'Métiers disponible en inversant deux caractéristiques',
            state: 'swap',
            icon: 'sync',
            jobs: []
        } as JobAvailability;
        const availabilityKo = {
            title: 'Métiers non disponibles',
            state: 'ko',
            icon: 'close',
            jobs: []
        } as JobAvailability;
        return {availabilityOk, availabilitySwap, availabilityKo};
    }

    selectJob(job: Job | undefined) {
        if (!job) {
            this.jobChange.emit(job);
            return false;
        }
        this.jobChange.emit(job);
        return false;
    }

    getJobs() {
        this.jobService.getJobList().subscribe(
            jobs => {
                this.jobs = jobs;
                this.updateJobStates();
            }
        );
    }

    randomSelect(): void {
        const count = this.availabilityOk.jobs.length;
        const rnd = getRandomInt(1, count + 1);
        if (rnd > count) {
            this.selectJob(undefined);
        } else {
            this.selectJob(this.availabilityOk.jobs[rnd - 1]);
        }
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
        this.updateJobStates();
    }

    ngOnInit() {
        this.getJobs();
    }
}
