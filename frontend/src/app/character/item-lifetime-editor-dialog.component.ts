import {Component, Inject, OnInit} from '@angular/core';
import {IDurable} from '../api/shared';
import {MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA} from '@angular/material/legacy-dialog';


export interface ItemLifetimeEditorDialogData {
    lifetime?: IDurable;
}

@Component({
    templateUrl: './item-lifetime-editor-dialog.component.html',
    styleUrls: ['./item-lifetime-editor-dialog.component.scss']
})
export class ItemLifetimeEditorDialogComponent {
    public lifetime: IDurable;

    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: ItemLifetimeEditorDialogData,
    ) {
        this.lifetime = {...data.lifetime || {durationType: 'forever'}};
    }
}
