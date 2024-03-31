import {Component, Inject, OnInit} from '@angular/core';
import {MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA} from '@angular/material/legacy-dialog';
import {ActiveStatsModifier} from '../shared';

export interface ModifierDetailsDialogData {
    readonly modifier: ActiveStatsModifier;
}

@Component({
    selector: 'app-modifier-details-dialog',
    templateUrl: './modifier-details-dialog.component.html',
    styleUrls: ['./modifier-details-dialog.component.scss']
})
export class ModifierDetailsDialogComponent implements OnInit {

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: ModifierDetailsDialogData
    ) {
    }

    ngOnInit() {
    }

}
