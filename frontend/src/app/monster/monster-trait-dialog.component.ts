import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {MonsterTrait} from './monster.model';

export interface MonsterTraitDialogData {
    trait: MonsterTrait;
}

@Component({
    templateUrl: './monster-trait-dialog.component.html',
    styleUrls: ['./monster-trait-dialog.component.scss'],
    standalone: false
})
export class MonsterTraitDialogComponent implements OnInit {

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: MonsterTraitDialogData
    ) {
    }

    ngOnInit() {
    }

}
