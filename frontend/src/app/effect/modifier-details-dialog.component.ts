import {Component, Inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import {ActiveStatsModifier} from '../shared';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { I18nPluralPipe } from '@angular/common';
import { ModifierPipe } from '../shared/modifier.pipe';
import { NhbkDateDurationPipe } from '../date/nhbk-duration.pipe';

export interface ModifierDetailsDialogData {
    readonly modifier: ActiveStatsModifier;
}

@Component({
    selector: 'app-modifier-details-dialog',
    templateUrl: './modifier-details-dialog.component.html',
    styleUrls: ['./modifier-details-dialog.component.scss'],
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, MatIcon, MatDialogActions, MatButton, MatDialogClose, I18nPluralPipe, ModifierPipe, NhbkDateDurationPipe]
})
export class ModifierDetailsDialogComponent implements OnInit {

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: ModifierDetailsDialogData
    ) {
    }

    ngOnInit() {
    }

}
