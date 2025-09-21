import {Component, Inject} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogClose } from '@angular/material/dialog';
import {Origin} from '../origin';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { OriginPlayerInfoComponent } from '../origin/origin-player-info.component';

export interface OriginPlayerDialogData {
    origin: Origin;
}

@Component({
    templateUrl: './origin-player-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './origin-player-dialog.component.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon, OriginPlayerInfoComponent]
})
export class OriginPlayerDialogComponent {

    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: OriginPlayerDialogData
    ) {
    }
}
