import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {NotificationsService} from '../notifications';
import {NhbkDialogService} from '../shared';

import {Item, ItemService} from '../item';
import {Loot, LootPanelComponent} from '../loot';
import {Monster} from '../monster';

import {Group} from './group.model';
import {GroupService} from './group.service';

@Component({
    selector: 'group-loot-panel',
    styleUrls: ['./group-loot-panel.component.scss'],
    templateUrl: './group-loot-panel.component.html',
})
export class GroupLootPanelComponent extends LootPanelComponent implements OnInit {
    @Input() group: Group;
    public newLootName: string|null;

    @ViewChild('addLootDialog', {static: true})
    public addLootDialog: Portal<any>;
    public addLootOverlayRef: OverlayRef;

    constructor(private notification: NotificationsService
        , private _groupService: GroupService
        , private _nhbkDialogService: NhbkDialogService
        , private _itemService: ItemService) {
        super(notification);
    }

    openAddLootDialog() {
        this.addLootOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.addLootDialog);
    }

    closeAddLootDialog() {
        this.addLootOverlayRef.detach();
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
        this.closeAddLootDialog();
    }

    deleteLoot(loot: Loot) {
        this._groupService.deleteLoot(loot.id).subscribe(
            () => {
                this.lootDeleted(loot.id);
            }
        );
    }

    onAddItem(data: {monster: Monster, loot: Loot, item: Item}) {
        if (data.loot) {
            this._itemService.addItemTo('loot', data.loot.id, data.item.template.id, data.item.data).subscribe(
                item => {
                    data.loot.addItem(item);
                }
            );
        }
        else {
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
