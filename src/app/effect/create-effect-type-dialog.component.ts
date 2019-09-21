import {Component, OnInit} from '@angular/core';
import {MatDialogRef} from '@angular/material/dialog';
import {FormControl, FormGroup, Validators} from '@angular/forms';

export interface CreateEffectTypeDialogResult {
    name: string;
    diceSize: number;
    diceCount: number;
    note?: string;
}

@Component({
    selector: 'app-create-effect-type-dialog',
    templateUrl: './create-effect-type-dialog.component.html',
    styleUrls: ['./create-effect-type-dialog.component.scss']
})
export class CreateEffectTypeDialogComponent implements OnInit {
    form: FormGroup = new FormGroup({
        name: new FormControl(),
        diceSize: new FormControl(),
        diceCount: new FormControl(),
        note: new FormControl(undefined, Validators.required)
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
