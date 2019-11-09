import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {Monster} from '../monster';
import {Item, ItemService} from '../item';
import {ItemDialogComponent} from '../item/item-dialog.component';
import {NhbkMatDialog} from '../material-workaround';

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
        private readonly dialog: NhbkMatDialog,
        @Inject(MAT_DIALOG_DATA) public readonly data: MonsterInventoryDialogData,
        private readonly itemService: ItemService,
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
