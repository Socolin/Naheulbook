import {Component, Inject, OnInit} from '@angular/core';
import {MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA, MatLegacyDialogRef as MatDialogRef} from '@angular/material/legacy-dialog';

import {UsefulDataDialogResult} from './useful-data-dialog-result';

export interface EffectListDialogData {
    inputSubCategoryId: number
}

@Component({
    templateUrl: './effect-list-dialog.component.html',
    styleUrls: ['../../shared/full-screen-dialog.scss', './effect-list-dialog.component.scss']
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
