import {Component, Inject} from '@angular/core';
import {Job} from '../job';
import { MAT_DIALOG_DATA, MatDialogClose } from '@angular/material/dialog';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { JobPlayerInfoComponent } from '../job/job-player-info.component';

export interface JobPlayerDialogData {
    job: Job;
}

@Component({
    templateUrl: './job-player-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './job-player-dialog.component.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon, JobPlayerInfoComponent]
})
export class JobPlayerDialogComponent {

    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: JobPlayerDialogData
    ) {
    }
}
