import {Component, OnInit} from '@angular/core';
import { MatDialogRef, MatDialogTitle, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { UntypedFormControl, UntypedFormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { CdkTextareaAutosize } from '@angular/cdk/text-field';
import { MatButton } from '@angular/material/button';

export interface CreateEffectTypeDialogResult {
    name: string;
    diceSize: number;
    diceCount: number;
    note?: string;
}

@Component({
    selector: 'app-create-effect-type-dialog',
    templateUrl: './create-effect-type-dialog.component.html',
    styleUrls: ['./create-effect-type-dialog.component.scss'],
    imports: [MatDialogTitle, MatDialogActions, FormsModule, ReactiveFormsModule, MatFormField, MatInput, CdkTextareaAutosize, MatButton, MatDialogClose]
})
export class CreateEffectTypeDialogComponent implements OnInit {
    form: UntypedFormGroup = new UntypedFormGroup({
        name: new UntypedFormControl(),
        diceSize: new UntypedFormControl(),
        diceCount: new UntypedFormControl(),
        note: new UntypedFormControl(undefined, Validators.required)
    });

    constructor(
        public dialogRef: MatDialogRef<CreateEffectTypeDialogComponent, CreateEffectTypeDialogResult>,
    ) {
    }

    ngOnInit() {
    }

    valid() {
        this.dialogRef.close(this.form.value);
    }
}
