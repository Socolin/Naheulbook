import {Component, Inject} from '@angular/core';
import {Job} from '../job';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';

export interface JobPlayerDialogData {
    job: Job;
}

@Component({
    templateUrl: './job-player-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './job-player-dialog.component.scss'],
    standalone: false
})
export class JobPlayerDialogComponent {

    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: JobPlayerDialogData
    ) {
    }
}
