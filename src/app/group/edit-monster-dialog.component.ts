import {Component, Inject, OnInit} from '@angular/core';
import {FormControl, FormGroup} from '@angular/forms';
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
    public readonly form: FormGroup;

    constructor(
        public dialogRef: MatDialogRef<EditMonsterDialogComponent, EditMonsterDialogResult>,
        @Inject(MAT_DIALOG_DATA) public data: EditMonsterDialogData
    ) {
        this.form = new FormGroup({
            name: new FormControl(data.monster.name),
            at: new FormControl(data.monster.data.at),
            prd: new FormControl(data.monster.data.prd),
            esq: new FormControl(data.monster.data.esq),
            ev: new FormControl(data.monster.data.ev),
            maxEv: new FormControl(data.monster.data.maxEv),
            ea: new FormControl(data.monster.data.ea),
            maxEa: new FormControl(data.monster.data.maxEa),
            pr: new FormControl(data.monster.data.pr),
            pr_magic: new FormControl(data.monster.data.pr_magic),
            dmg: new FormControl(data.monster.data.dmg),
            cou: new FormControl(data.monster.data.cou),
            chercheNoise: new FormControl(data.monster.data.chercheNoise),
            resm: new FormControl(data.monster.data.resm),
            xp: new FormControl(data.monster.data.xp),
            note: new FormControl(data.monster.data.note),
            sex: new FormControl(data.monster.data.sex),
        });
    }


    edit() {
        this.dialogRef.close(this.form.getRawValue())
    }

    ngOnInit() {
    }
}
