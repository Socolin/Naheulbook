import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {Character} from './character.model';
import {Job} from '../job';

export interface ChangeJobDialogData {
    readonly character: Character
}

export interface ChangeJobDialogResult {
    readonly addedJobs: Job[];
    readonly deletedJobs: Job[];
}

@Component({
    templateUrl: './change-job-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './change-job-dialog.component.scss'],
    standalone: false
})
export class ChangeJobDialogComponent {
    public jobs: readonly Job[];

    constructor(
        private readonly dialogRef: MatDialogRef<ChangeJobDialogComponent, ChangeJobDialogResult>,
        @Inject(MAT_DIALOG_DATA) public readonly data: ChangeJobDialogData,
    ) {
        this.jobs = [...data.character.jobs]
    }

    saveJobChange() {
        this.dialogRef.close({
            addedJobs: this.jobs.filter(j => !this.data.character.jobs.find(cj => cj.id === j.id)),
            deletedJobs: this.data.character.jobs.filter(j => !this.jobs.find(cj => cj.id === j.id))
        });
    }

    addJob(job: Job) {
        const index = this.jobs.findIndex(j => j.id === job.id);
        if (index === -1) {
            this.jobs = [...this.jobs, job];
        }
    }

    removeJob(job: Job) {
        this.jobs = [...this.jobs.filter(j => j.id !== job.id)];
    }
}
