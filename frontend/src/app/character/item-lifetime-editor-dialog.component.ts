import {Component, Inject, OnInit} from '@angular/core';
import {IDurable} from '../api/shared';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';


export interface ItemLifetimeEditorDialogData {
    lifetime?: IDurable;
}

@Component({
    templateUrl: './item-lifetime-editor-dialog.component.html',
    styleUrls: ['./item-lifetime-editor-dialog.component.scss'],
    standalone: false
})
export class ItemLifetimeEditorDialogComponent {
    public lifetime: IDurable;

    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: ItemLifetimeEditorDialogData,
    ) {
        this.lifetime = {...data.lifetime || {durationType: 'forever'}};
    }
}
