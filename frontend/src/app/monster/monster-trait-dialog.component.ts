import {Component, Inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import {MonsterTrait} from './monster.model';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatButton } from '@angular/material/button';

export interface MonsterTraitDialogData {
    trait: MonsterTrait;
    /** When set, only the description of this level (1-based) is shown */
    level?: number;
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

    public levels: { level: number, description: string }[] = [];

    ngOnInit() {
        const levels = (this.data.trait.levels || []).map((description, i) => ({level: i + 1, description}));
        if (this.data.level) {
            this.levels = levels.filter(l => l.level === this.data.level);
        } else {
            this.levels = levels;
        }
    }

}
