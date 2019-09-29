import {Component, Input, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications';

import {Item, ItemService} from '../item';
import {Loot, LootPanelComponent} from '../loot';
import {Monster} from '../monster';

import {Group} from './group.model';
import {GroupService} from './group.service';
import {MatDialog} from '@angular/material/dialog';
import {openCreateItemDialog} from './create-item-dialog.component';
import {AddLootDialogComponent} from './add-loot-dialog.component';
import {WebSocketService} from '../websocket';
import {openCreateGemDialog} from './create-gem-dialog.component';
import {ItemDialogComponent} from '../item/item-dialog.component';

@Component({
    selector: 'group-loot-panel',
    styleUrls: ['./group-loot-panel.component.scss'],
    templateUrl: './group-loot-panel.component.html',
})
export class GroupLootPanelComponent extends LootPanelComponent implements OnInit {
    @Input() group: Group;

    constructor(
        private readonly notification: NotificationsService,
        private readonly groupService: GroupService,
        private readonly itemService: ItemService,
        private readonly dialog: MatDialog,
        private readonly websocketService: WebSocketService,
    ) {
        super(notification, websocketService);
    }

    openAddItemDialog(target: Loot|Monster) {
        openCreateItemDialog(this.dialog, (item) => {
            if (target instanceof Loot) {
                this.onAddItem({loot: target, item: item});
            } else {
                this.onAddItem({monster: target, item: item});
            }
        });
    }

    openAddGemDialog(target: Loot|Monster) {
        openCreateGemDialog(this.dialog, (item) => {
            if (target instanceof Loot) {
                this.onAddItem({loot: target, item: item});
            } else {
                this.onAddItem({monster: target, item: item});
            }
        });
    }

    openAddLootDialog() {
        const dialogRef = this.dialog.open(AddLootDialogComponent);
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            this.groupService.createLoot(this.group.id, result).subscribe(
                loot => {
                    this.lootAdded(loot, false);
                }
            );
        })
    }

    deleteLoot(loot: Loot) {
        this.groupService.deleteLoot(loot.id).subscribe(
            () => {
                this.lootDeleted(loot.id);
            }
        );
    }

    onAddItem(data: { loot: Loot, item: Item } | { monster: Monster, item: Item }) {
        if ('loot' in data) {
            const loot = data.loot;
            this.itemService.addItemTo('loot', data.loot.id, data.item.template.id, data.item.data).subscribe(
                item => {
                    loot.addItem(item);
                }
            );
        } else {
            const monster = data.monster;
            this.itemService.addItemTo('monster', data.monster.id, data.item.template.id, data.item.data).subscribe(
                item => {
                    monster.addItem(item);
                }
            );
        }
    }

    openLoot(loot: Loot) {
        this.groupService.updateLootVisibility(loot.id, true).subscribe(
            () => {
                loot.visibleForPlayer = true;
            }
        );
    }

    closeLoot(loot: Loot) {
        this.groupService.updateLootVisibility(loot.id, false).subscribe(
            () => {
                loot.visibleForPlayer = false;
            }
        );
    }

    addRandomItemFromCategoryToLoot(loot: Loot, categoryName: string) {
        this.itemService.addRandomItemTo('loot', loot.id, categoryName).subscribe(
            item => {
                loot.addItem(item);
            }
        );
        return false;
    }

    addRandomItemFromCategoryToMonster(monster: Monster, categoryName: string) {
        this.itemService.addRandomItemTo('monster', monster.id, categoryName).subscribe(
            item => {
                monster.addItem(item);
            }
        );
        return false;
    }

    removeItemFromLoot(loot: Loot, item: Item) {
        this.itemService.deleteItem(item.id).subscribe(
            () => {
                loot.removeItem(item.id);
            }
        );
    }

    removeItemFromMonster(monster: Monster, item: Item) {
        this.itemService.deleteItem(item.id).subscribe(
            () => {
                monster.removeItem(item.id);
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

    ngOnInit() {
        this.loots = this.group.loots;
    }
}
