import {Component, Input, OnInit} from '@angular/core';

import {NotificationsService} from "../notifications/notifications.service";

import {GroupService} from "./group.service";
import {GroupWebsocketService} from "./group.websocket.service";
import {Group} from "./group.model";
import {ItemService} from "../item/item.service";
import {Item} from "../character/item.model";
import {LootPanelComponent} from "../loot/loot-panel.component";
import {LootWebsocketService} from "../loot/loot.websocket.service";
import {Loot} from "../loot/loot.model";
import {Monster} from "../monster/monster.model";

@Component({
    selector: 'group-loot-panel',
    templateUrl: 'group-loot-panel.component.html',
    providers: [LootWebsocketService],
})
export class GroupLootPanelComponent extends LootPanelComponent implements OnInit {
    @Input() group: Group;
    public lootTargetForNewItem: Loot;
    public monsterTargetForNewItem: Monster;
    public newLootName: string;

    constructor(private lootWebsocketService: LootWebsocketService
        , private notification: NotificationsService
        , private _groupService: GroupService
        , private _groupWebsocketService: GroupWebsocketService
        , private _itemService: ItemService) {
        super(lootWebsocketService, notification);
    }

    createLoot() {
        this._groupService.createLoot(this.group.id, this.newLootName).subscribe(
            loot => {
                this.onAddLoot(loot);
            }
        );
        this.newLootName = null;
    }

    selectLootToAddItem(loot: Loot) {
        this.lootTargetForNewItem = loot;
    }

    selectMonsterToAddItem(loot: Loot, monster: Monster) {
        this.lootTargetForNewItem = loot;
        this.monsterTargetForNewItem = monster;
    }

    deleteLoot(loot: Loot) {
        this._groupService.deleteLoot(loot.id).subscribe(
            loot => {
                this.onDeleteLoot(loot);
            }
        );
    }

    openLoot(loot: Loot) {
        loot.visibleForPlayer = true;
        this._groupService.updateLoot(loot).subscribe(
            loot => {
                this.onUpdateLoot(loot);
            }
        );
    }

    closeLoot(loot: Loot) {
        loot.visibleForPlayer = false;
        this._groupService.updateLoot(loot).subscribe(
            loot => {
                this.onUpdateLoot(loot);
            }
        );
    }

    addRandomItemFromCategoryToLoot(loot: Loot,categoryName: string) {
        this._itemService.addRandomItemTo('loot', loot.id, {categoryName: categoryName}).subscribe(
            item => {
                this.onAddItemToLoot(loot, item);
            }
        );
        return false;
    }

    addRandomItemFromCategoryToMonster(loot: Loot, monster: Monster,categoryName: string) {
        this._itemService.addRandomItemTo('monster', monster.id, {categoryName: categoryName}).subscribe(
            item => {
                this.onAddItemToMonster(loot, {monster, item});
            }
        );
        return false;
    }

    addItemToLoot(loot: Loot, item: Item) {
        if (item != null) {
            this._itemService.addItemTo('loot', loot.id, item.template.id, item.data).subscribe(
                item => {
                    this.onAddItemToLoot(loot, item);
                }
            );
        }
        this.lootTargetForNewItem = null;
    }

    addItemToMonster(loot: Loot, monster: Monster, item: Item) {
        if (item != null) {
            this._itemService.addItemTo('monster', monster.id, item.template.id, item.data).subscribe(
                item => {
                    this.onAddItemToMonster(loot, {item: item, monster: monster});
                }
            );
        }
        this.lootTargetForNewItem = null;
        this.monsterTargetForNewItem = null;
    }

    removeItemFromLoot(loot: Loot, item: Item) {
        if (item != null) {
            this._itemService.deleteItem(item.id).subscribe(
                item => {
                    this.onRemoveItemFromLoot(loot, item);
                }
            );
        }
    }

    removeItemFromMonster(monster: Monster, item: Item) {
        if (item != null) {
            this._itemService.deleteItem(item.id).subscribe(
                item => {
                    this.onRemoveItemFromMonster(monster, item);
                }
            );
        }
    }

    registerActions() {
        this._groupWebsocketService.registerPacket("addLoot").subscribe(this.onAddLoot.bind(this));
        this._groupWebsocketService.registerPacket("deleteLoot").subscribe(this.onDeleteLoot.bind(this));
        this._groupWebsocketService.registerPacket("updateLoot").subscribe(this.onUpdateLoot.bind(this));
    }

    ngOnInit(): void {
        this._groupService.loadLoots(this.group.id).subscribe(
            loots => {
                this.onLoadLoots(loots);
                this.registerActions();
                this._lootWebsocketService.registerNotifyFunction(null);
            }
        );
    }
}
