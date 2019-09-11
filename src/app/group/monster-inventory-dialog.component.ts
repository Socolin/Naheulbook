import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialog} from '@angular/material/dialog';
import {Monster} from '../monster';
import {Item, ItemService} from '../item';
import {ItemTemplate} from '../item-template';
import {ItemTemplateDialogComponent} from '../item-template/item-template-dialog.component';
import {ItemDialogComponent} from '../item/item-dialog.component';

export interface MonsterInventoryDialogData {
    monster: Monster;
}

@Component({
    selector: 'app-monster-inventory-dialog',
    templateUrl: './monster-inventory-dialog.component.html',
    styleUrls: ['./monster-inventory-dialog.component.scss']
})
export class MonsterInventoryDialogComponent {

    constructor(
        private itemService: ItemService,
        private dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: MonsterInventoryDialogData,
    ) {
    }

    removeItem(item: Item) {
        this.itemService.deleteItem(item.id).subscribe(
            () => {
                this.data.monster.removeItem(item.id);
            }
        );
    }

    openItemDialog(item: Item) {
        this.dialog.open(ItemDialogComponent, {
            panelClass: 'app-dialog-no-padding',
            data: {item},
            autoFocus: false
        });
    }
}
