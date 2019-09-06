import {Component, Input, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications';

import {Item, ItemService} from '../item';
import {Loot, LootPanelComponent} from '../loot';
import {Monster} from '../monster';

import {Group} from './group.model';
import {GroupService} from './group.service';
import {MatDialog} from '@angular/material/dialog';
import {CreateItemDialogComponent} from './create-item-dialog.component';
import {Subject} from 'rxjs';

@Component({
    selector: 'group-loot-panel',
    styleUrls: ['./group-loot-panel.component.scss'],
    templateUrl: './group-loot-panel.component.html',
})
export class GroupLootPanelComponent extends LootPanelComponent implements OnInit {
    @Input() group: Group;
    public newLootName: string | null;

    constructor(private notification: NotificationsService
        , private _groupService: GroupService
        , private _itemService: ItemService
        , private dialog: MatDialog
    ) {
        super(notification);
    }

    openAddItemDialog(loot: Loot) {
        const subject = new Subject<Item>();
        const dialogRef = this.dialog.open(CreateItemDialogComponent, {data: {onAdd: subject}});
        const subscription = subject.subscribe((item) => {
            this.onAddItem({loot: loot, item: item});
        });

        dialogRef.afterClosed().subscribe((result) => {
            subscription.unsubscribe();
            if (!result) {
                return;
            }
            this.onAddItem({loot: loot, item: result});
        });

    }

    openAddLootDialog() {
    }

    createLoot() {
        if (!this.newLootName) {
            return;
        }
        this._groupService.createLoot(this.group.id, this.newLootName).subscribe(
            loot => {
                this.lootAdded(loot, false);
            }
        );
        this.newLootName = null;
    }

    deleteLoot(loot: Loot) {
        this._groupService.deleteLoot(loot.id).subscribe(
            () => {
                this.lootDeleted(loot.id);
            }
        );
    }

    onAddItem(data: { monster?: Monster, loot?: Loot, item: Item }) {
        if (data.loot) {
            this._itemService.addItemTo('loot', data.loot.id, data.item.template.id, data.item.data).subscribe(
                item => {
                    data.loot.addItem(item);
                }
            );
        } else {
            this._itemService.addItemTo('monster', data.monster.id, data.item.template.id, data.item.data).subscribe(
                item => {
                    data.monster.addItem(item);
                }
            );
        }
    }

    openLoot(loot: Loot) {
        this._groupService.updateLootVisibility(loot.id, true).subscribe(
            () => {
                loot.visibleForPlayer = true;
            }
        );
    }

    closeLoot(loot: Loot) {
        this._groupService.updateLootVisibility(loot.id, false).subscribe(
            () => {
                loot.visibleForPlayer = false;
            }
        );
    }

    addRandomItemFromCategoryToLoot(loot: Loot, categoryName: string) {
        this._itemService.addRandomItemTo('loot', loot.id, categoryName).subscribe(
            item => {
                loot.addItem(item);
            }
        );
        return false;
    }

    addRandomItemFromCategoryToMonster(monster: Monster, categoryName: string) {
        this._itemService.addRandomItemTo('monster', monster.id, categoryName).subscribe(
            item => {
                monster.addItem(item);
            }
        );
        return false;
    }

    removeItemFromLoot(loot: Loot, item: Item) {
        if (item != null) {
            this._itemService.deleteItem(item.id).subscribe(
                () => {
                    loot.removeItem(item.id);
                }
            );
        }
    }

    removeItemFromMonster(monster: Monster, item: Item) {
        if (item != null) {
            this._itemService.deleteItem(item.id).subscribe(
                () => {
                    monster.removeItem(item.id);
                }
            );
        }
    }

    ngOnInit() {
        this.loots = this.group.loots;
    }

    openAddGemDialog(loot: Loot) {
        // FIXME
    }
}
