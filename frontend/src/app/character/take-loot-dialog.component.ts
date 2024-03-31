import {Component, Inject, OnInit} from '@angular/core';
import {MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA, MatLegacyDialogRef as MatDialogRef} from '@angular/material/legacy-dialog';
import {Item} from '../item';

export interface TakeLootDialogData {
    item: Item;
}

export interface TakeLootDialogResult {
    quantity?: number;
}

@Component({
    templateUrl: './take-loot-dialog.component.html',
    styleUrls: ['./take-loot-dialog.component.scss']
})
export class TakeLootDialogComponent implements OnInit {
    public takingQuantity?: number;

    constructor(
        private readonly dialogRef: MatDialogRef<TakeLootDialogComponent, TakeLootDialogResult>,
        @Inject(MAT_DIALOG_DATA) public readonly data: TakeLootDialogData,
    ) {
        if (data.item.template.data.quantifiable) {
            this.takingQuantity = data.item.data.quantity;
        } else {
            this.takingQuantity = undefined;
        }
    }

    ngOnInit() {
    }

    takeItem() {
        this.dialogRef.close({quantity: this.takingQuantity});
    }
}
