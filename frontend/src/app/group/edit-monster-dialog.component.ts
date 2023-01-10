import {Component, Inject, OnInit} from '@angular/core';
import {UntypedFormControl, UntypedFormGroup} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {Monster} from '../monster';

export interface EditMonsterDialogData {
    monster: Monster
}

export interface EditMonsterDialogResult {
    name: string;
    at: number;
    prd: number;
    esq: number;
    ev: number;
    maxEv: number;
    ea: number;
    maxEa: number;
    pr: number;
    pr_magic: number;
    dmg: string;
    cou: number;
    chercheNoise: boolean;
    resm: number;
    xp: number;
    note: string;
    sex: string;
}

@Component({
    selector: 'app-edit-monster-dialog',
    templateUrl: './edit-monster-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './edit-monster-dialog.component.scss']
})
export class EditMonsterDialogComponent implements OnInit {
    public readonly form: UntypedFormGroup;

    constructor(
        public dialogRef: MatDialogRef<EditMonsterDialogComponent, EditMonsterDialogResult>,
        @Inject(MAT_DIALOG_DATA) public data: EditMonsterDialogData
    ) {
        this.form = new UntypedFormGroup({
            name: new UntypedFormControl(data.monster.name),
            at: new UntypedFormControl(data.monster.data.at),
            prd: new UntypedFormControl(data.monster.data.prd),
            esq: new UntypedFormControl(data.monster.data.esq),
            ev: new UntypedFormControl(data.monster.data.ev),
            maxEv: new UntypedFormControl(data.monster.data.maxEv),
            ea: new UntypedFormControl(data.monster.data.ea),
            maxEa: new UntypedFormControl(data.monster.data.maxEa),
            pr: new UntypedFormControl(data.monster.data.pr),
            pr_magic: new UntypedFormControl(data.monster.data.pr_magic),
            dmg: new UntypedFormControl(data.monster.data.dmg),
            cou: new UntypedFormControl(data.monster.data.cou),
            chercheNoise: new UntypedFormControl(data.monster.data.chercheNoise),
            resm: new UntypedFormControl(data.monster.data.resm),
            xp: new UntypedFormControl(data.monster.data.xp),
            note: new UntypedFormControl(data.monster.data.note),
            sex: new UntypedFormControl(data.monster.data.sex),
        });
    }


    edit() {
        this.dialogRef.close(this.form.getRawValue())
    }

    ngOnInit() {
    }
}
