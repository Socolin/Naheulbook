import {Component, OnInit} from '@angular/core';
import {MatLegacyDialogRef as MatDialogRef} from '@angular/material/legacy-dialog';
import {UsefulDataDialogResult} from './useful-data-dialog-result';

@Component({
    templateUrl: './item-templates-dialog.component.html',
    styleUrls: ['../../shared/full-screen-dialog.scss', './item-templates-dialog.component.scss']
})
export class ItemTemplatesDialogComponent implements OnInit {

    constructor(
        private readonly dialogRef: MatDialogRef<ItemTemplatesDialogComponent, UsefulDataDialogResult>,
    ) {
    }

    ngOnInit() {
    }

    onAction(action: { action: string; data: any }) {
        this.dialogRef.close({action})
    }
}
