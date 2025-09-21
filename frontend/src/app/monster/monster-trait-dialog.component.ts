import {Component, Inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import {MonsterTrait} from './monster.model';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatButton } from '@angular/material/button';

export interface MonsterTraitDialogData {
    trait: MonsterTrait;
}

@Component({
    templateUrl: './monster-trait-dialog.component.html',
    styleUrls: ['./monster-trait-dialog.component.scss'],
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, MatDialogActions, MatButton, MatDialogClose]
})
export class MonsterTraitDialogComponent implements OnInit {

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: MonsterTraitDialogData
    ) {
    }

    ngOnInit() {
    }

}
