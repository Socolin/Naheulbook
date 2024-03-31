import {Component, OnInit} from '@angular/core';
import {ActiveStatsModifier} from '../shared';
import { MatLegacyDialogRef as MatDialogRef } from '@angular/material/legacy-dialog';

@Component({
    selector: 'app-add-item-modifier-dialog',
    templateUrl: './add-item-modifier-dialog.component.html',
    styleUrls: ['./add-item-modifier-dialog.component.scss', '../shared/full-screen-dialog.scss']
})
export class AddItemModifierDialogComponent implements OnInit {

    public newItemModifier: ActiveStatsModifier = new ActiveStatsModifier();

    constructor(
        public dialogRef: MatDialogRef<AddItemModifierDialogComponent>,
    ) {
    }

    ngOnInit() {
    }

    valid() {
        this.dialogRef.close(this.newItemModifier);
    }
}
