import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';

export enum ValueEditorModes {
    keyboard = 'keyboard',
    mobile = 'mobile'
}

export class ValueEditorSettingsDialogData {
    mode?: ValueEditorModes
}

export class ValueEditorSettingsDialogResult {
    mode?: ValueEditorModes
}

@Component({
    selector: 'app-value-editor-settings-dialog',
    templateUrl: './value-editor-settings-dialog.component.html',
    styleUrls: ['./value-editor-settings-dialog.component.scss'],
    standalone: false
})
export class ValueEditorSettingsDialogComponent implements OnInit {
    modes = ValueEditorModes;
    mode?: ValueEditorModes;

    constructor(
        public dialogRef: MatDialogRef<ValueEditorSettingsDialogComponent, ValueEditorSettingsDialogResult>,
        @Inject(MAT_DIALOG_DATA) public data: ValueEditorSettingsDialogData) {
        this.mode = data.mode;
    }

    ngOnInit(): void {
    }

    save() {
        this.dialogRef.close({
            mode: this.mode
        });
    }
}
