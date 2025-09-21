import {Component, Inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogClose } from '@angular/material/dialog';

import {UsefulDataDialogResult} from './useful-data-dialog-result';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { EffectListComponent } from '../../effect/effect-list.component';

export interface EffectListDialogData {
    inputSubCategoryId: number
}

@Component({
    templateUrl: './effect-list-dialog.component.html',
    styleUrls: ['../../shared/full-screen-dialog.scss', './effect-list-dialog.component.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon, EffectListComponent]
})
export class EffectListDialogComponent implements OnInit {

    constructor(
        private readonly dialogRef: MatDialogRef<EffectListDialogComponent, UsefulDataDialogResult>,
        @Inject(MAT_DIALOG_DATA) public readonly data: EffectListDialogData,
    ) {
    }

    ngOnInit() {
    }

    onAction(action: { action: string; data: any }) {
        this.dialogRef.close({action})
    }
}
