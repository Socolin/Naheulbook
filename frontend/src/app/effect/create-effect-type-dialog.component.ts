import {Component, OnInit} from '@angular/core';
import {MatDialogRef} from '@angular/material/dialog';
import {UntypedFormControl, UntypedFormGroup, Validators} from '@angular/forms';

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
    standalone: false
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
