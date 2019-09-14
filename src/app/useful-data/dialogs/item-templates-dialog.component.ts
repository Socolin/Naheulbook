import {Component, OnInit} from '@angular/core';
import {MatDialogRef} from '@angular/material/dialog';
import {UsefulDataDialogResult} from './useful-data-dialog-result';

@Component({
    templateUrl: './item-templates-dialog.component.html',
    styleUrls: ['../../shared/full-screen-dialog.scss', './item-templates-dialog.component.scss']
})
export class ItemTemplatesDialogComponent implements OnInit {

    constructor(
        private dialogRef: MatDialogRef<ItemTemplatesDialogComponent, UsefulDataDialogResult>,
    ) {
    }

    ngOnInit() {
    }

    onAction(action: { action: string; data: any }) {
        this.dialogRef.close({action})
    }
}
