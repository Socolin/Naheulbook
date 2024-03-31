import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {Origin} from '../origin';

export interface OriginPlayerDialogData {
    origin: Origin;
}

@Component({
    templateUrl: './origin-player-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './origin-player-dialog.component.scss']
})
export class OriginPlayerDialogComponent {

    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: OriginPlayerDialogData
    ) {
    }
}
