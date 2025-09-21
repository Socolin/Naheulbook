import {Component, Inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatRadioGroup, MatRadioButton } from '@angular/material/radio';
import { FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';

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
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, MatRadioGroup, FormsModule, MatRadioButton, MatDialogActions, MatButton, MatDialogClose]
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
