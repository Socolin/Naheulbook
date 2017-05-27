import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {OverlayRef, Portal} from '@angular/material';

import {NotificationsService} from '../notifications/notifications.service';

import {GroupService} from './group.service';
import {GroupWebsocketService} from './group.websocket.service';
import {Group} from './group.model';
import {ItemService} from '../item/item.service';
import {Item} from '../character/item.model';
import {LootPanelComponent} from '../loot/loot-panel.component';
import {LootWebsocketService} from '../loot/loot.websocket.service';
import {Loot} from '../loot/loot.model';
import {Monster} from '../monster/monster.model';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';

@Component({
    selector: 'group-loot-panel',
    styleUrls: ['./group-loot-panel.component.scss'],
    templateUrl: './group-loot-panel.component.html',
    providers: [LootWebsocketService],
})
export class GroupLootPanelComponent extends LootPanelComponent implements OnInit {
    @Input() group: Group;
    public newLootName: string;

    @ViewChild('addLootDialog')
    public addLootDialog: Portal<any>;
    public addLootOverlayRef: OverlayRef;

    constructor(private lootWebsocketService: LootWebsocketService
        , private notification: NotificationsService
        , private _groupService: GroupService
        , private _nhbkDialogService: NhbkDialogService
        , private _groupWebsocketService: GroupWebsocketService
        , private _itemService: ItemService) {
        super(lootWebsocketService, notification);
    }

    openAddLootDialog() {
        this.addLootOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.addLootDialog);
    }

    closeAddLootDialog() {
        this.addLootOverlayRef.detach();
    }

    createLoot() {
        this._groupService.createLoot(this.group.id, this.newLootName).subscribe(
            loot => {
                this.lootAdded(loot);
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
        loot.visibleForPlayer = true;
        this._groupService.updateLoot(loot).subscribe(
            updatedLoot => {
                loot.visibleForPlayer = updatedLoot.visibleForPlayer;
            }
        );
    }

    closeLoot(loot: Loot) {
        loot.visibleForPlayer = false;
        this._groupService.updateLoot(loot).subscribe(
            updatedLoot => {
                loot.visibleForPlayer = updatedLoot.visibleForPlayer;
            }
        );
    }

    addRandomItemFromCategoryToLoot(loot: Loot, categoryName: string) {
        this._itemService.addRandomItemTo('loot', loot.id, {categoryName: categoryName}).subscribe(
            item => {
                loot.addItem(item);
            }
        );
        return false;
    }

    addRandomItemFromCategoryToMonster(monster: Monster, categoryName: string) {
        this._itemService.addRandomItemTo('monster', monster.id, {categoryName: categoryName}).subscribe(
            item => {
                monster.addItem(item);
            }
        );
        return false;
    }

    removeItemFromLoot(loot: Loot, item: Item) {
        if (item != null) {
            this._itemService.deleteItem(item.id).subscribe(
                deletedItem => {
                    loot.removeItem(deletedItem.id);
                }
            );
        }
    }

    removeItemFromMonster(monster: Monster, item: Item) {
        if (item != null) {
            this._itemService.deleteItem(item.id).subscribe(
                deletedItem => {
                    monster.removeItem(deletedItem.id);
                }
            );
        }
    }

    registerActions() {
        this._groupWebsocketService.registerPacket('lootAdded').subscribe(this.lootAdded.bind(this));
        this._groupWebsocketService.registerPacket('deleteLoot').subscribe(this.lootDeleted.bind(this));
    }

    ngOnInit(): void {
        this._groupService.loadLoots(this.group.id).subscribe(
            loots => {
                this.onLoadLoots(loots);
                this.registerActions();
            }
        );
    }
}
