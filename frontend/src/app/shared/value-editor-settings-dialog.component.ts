import {Component, Inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatRipple} from '@angular/material/core';

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
    imports: [MatDialogTitle, MatDialogContent, MatDialogActions, MatButton, MatDialogClose, MatIcon, MatRipple]
})
export class ValueEditorSettingsDialogComponent implements OnInit {
    readonly options = [
        {
            mode: ValueEditorModes.mobile,
            icon: 'touch_app',
            title: 'Tactile',
            description: 'Un pavé de boutons (+1, +5, -50…), adapté aux écrans tactiles.'
        },
        {
            mode: ValueEditorModes.keyboard,
            icon: 'keyboard',
            title: 'Clavier',
            description: 'Un champ texte : saisissez la nouvelle valeur, ou une valeur relative commençant par + ou - (ex : +8, -16).'
        },
    ];
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
