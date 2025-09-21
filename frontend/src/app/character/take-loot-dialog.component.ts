import {Component, Inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import {Item} from '../item';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { IconComponent } from '../shared/icon.component';
import { ValueEditorComponent } from '../shared/value-editor.component';
import { MatButton } from '@angular/material/button';

export interface TakeLootDialogData {
    item: Item;
}

export interface TakeLootDialogResult {
    quantity?: number;
}

@Component({
    templateUrl: './take-loot-dialog.component.html',
    styleUrls: ['./take-loot-dialog.component.scss'],
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, IconComponent, ValueEditorComponent, MatDialogActions, MatButton, MatDialogClose]
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
