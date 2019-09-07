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
import {AddLootDialogComponent} from './add-loot-dialog.component';
import {WebSocketService} from '../websocket';
import {CreateGemDialogComponent} from './create-gem-dialog.component';

@Component({
    selector: 'group-loot-panel',
    styleUrls: ['./group-loot-panel.component.scss'],
    templateUrl: './group-loot-panel.component.html',
})
export class GroupLootPanelComponent extends LootPanelComponent implements OnInit {
    @Input() group: Group;

    constructor(private notification: NotificationsService
        , private _groupService: GroupService
        , private _itemService: ItemService
        , private dialog: MatDialog
        , private websocketService: WebSocketService
    ) {
        super(notification, websocketService);
    }

    openAddItemDialog(target: Loot|Monster) {
        const subject = new Subject<Item>();
        const dialogRef = this.dialog.open(CreateItemDialogComponent, {data: {onAdd: subject}});
        const subscription = subject.subscribe((item) => {
            if (target instanceof Loot) {
                this.onAddItem({loot: target, item: item});
            } else {
                this.onAddItem({monster: target, item: item});
            }
        });

        dialogRef.afterClosed().subscribe((result) => {
            subscription.unsubscribe();
        });
    }

    openAddGemDialog(target: Loot|Monster) {
        const subject = new Subject<Item>();
        const dialogRef = this.dialog.open(CreateGemDialogComponent, {data: {onAdd: subject}, autoFocus: false});
        const subscription = subject.subscribe((item) => {
            if (target instanceof Loot) {
                this.onAddItem({loot: target, item: item});
            } else {
                this.onAddItem({monster: target, item: item});
            }
        });

        dialogRef.afterClosed().subscribe((result) => {
            subscription.unsubscribe();
        });
    }

    openAddLootDialog() {
        const dialogRef = this.dialog.open(AddLootDialogComponent);
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            this._groupService.createLoot(this.group.id, result).subscribe(
                loot => {
                    this.lootAdded(loot, false);
                }
            );
        })
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
}
