import {Component, Inject, OnInit} from '@angular/core';
import {UntypedFormControl, UntypedFormGroup} from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogClose } from '@angular/material/dialog';
import {Monster} from '../monster';
import {MonsterAttackData} from '../api/shared';
import {cleanMonsterAttacks, createMonsterAttacksFormArray} from '../monster/monster-attacks-editor.component';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MonsterEditorComponent } from './monster-editor.component';

export interface EditMonsterDialogData {
    monster: Monster
}

export interface EditMonsterDialogResult {
    name: string;
    attacks: MonsterAttackData[];
    prd: number;
    esq: number;
    ev: number;
    maxEv: number;
    ea: number;
    maxEa: number;
    pr: number;
    pr_magic: number;
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
    styleUrls: ['../shared/full-screen-dialog.scss', './edit-monster-dialog.component.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon, MatButton, MonsterEditorComponent]
})
export class EditMonsterDialogComponent implements OnInit {
    public readonly form: UntypedFormGroup;

    constructor(
        public dialogRef: MatDialogRef<EditMonsterDialogComponent, EditMonsterDialogResult>,
        @Inject(MAT_DIALOG_DATA) public data: EditMonsterDialogData
    ) {
        this.form = new UntypedFormGroup({
            name: new UntypedFormControl(data.monster.name),
            attacks: createMonsterAttacksFormArray(data.monster.data.attacks),
            prd: new UntypedFormControl(data.monster.data.prd),
            esq: new UntypedFormControl(data.monster.data.esq),
            ev: new UntypedFormControl(data.monster.data.ev),
            maxEv: new UntypedFormControl(data.monster.data.maxEv),
            ea: new UntypedFormControl(data.monster.data.ea),
            maxEa: new UntypedFormControl(data.monster.data.maxEa),
            pr: new UntypedFormControl(data.monster.data.pr),
            pr_magic: new UntypedFormControl(data.monster.data.pr_magic),
            cou: new UntypedFormControl(data.monster.data.cou),
            chercheNoise: new UntypedFormControl(data.monster.data.chercheNoise),
            resm: new UntypedFormControl(data.monster.data.resm),
            xp: new UntypedFormControl(data.monster.data.xp),
            note: new UntypedFormControl(data.monster.data.note),
            sex: new UntypedFormControl(data.monster.data.sex),
        });
    }


    edit() {
        const value = this.form.getRawValue();
        this.dialogRef.close({...value, attacks: cleanMonsterAttacks(value.attacks)});
    }

    ngOnInit() {
    }
}
