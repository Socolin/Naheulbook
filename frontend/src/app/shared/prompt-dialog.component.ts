import {Component, Inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';

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
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, MatFormField, MatLabel, MatInput, FormsModule, MatDialogActions, MatButton, MatDialogClose]
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
