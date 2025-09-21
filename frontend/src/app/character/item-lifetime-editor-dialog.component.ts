import {Component, Inject, OnInit} from '@angular/core';
import {IDurable} from '../api/shared';
import { MAT_DIALOG_DATA, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { DurationSelectorComponent } from '../date/duration-selector.component';
import { MatButton } from '@angular/material/button';


export interface ItemLifetimeEditorDialogData {
    lifetime?: IDurable;
}

@Component({
    templateUrl: './item-lifetime-editor-dialog.component.html',
    styleUrls: ['./item-lifetime-editor-dialog.component.scss'],
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, DurationSelectorComponent, MatDialogActions, MatButton, MatDialogClose]
})
export class ItemLifetimeEditorDialogComponent {
    public lifetime: IDurable;

    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: ItemLifetimeEditorDialogData,
    ) {
        this.lifetime = {...data.lifetime || {durationType: 'forever'}};
    }
}
