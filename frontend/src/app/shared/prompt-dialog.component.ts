import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';

export interface PromptDialogData {
    title?: string;
    text?: string;
    placeholder?: string;
    confirmText: string;
    cancelText: string;
    initialValue?: string;
}

export interface PromptDialogResult {
    text: string;
}

@Component({
    templateUrl: './prompt-dialog.component.html',
    styleUrls: ['./prompt-dialog.component.scss'],
    standalone: false
})
export class PromptDialogComponent implements OnInit {

    constructor(
        public dialogRef: MatDialogRef<PromptDialogComponent, PromptDialogResult>,
        @Inject(MAT_DIALOG_DATA) public data: PromptDialogData
    ) {
    }

    confirm(text: string) {
        this.dialogRef.close({text})
    }

    ngOnInit() {
    }

}
