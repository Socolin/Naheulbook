import {Component, Input, OnInit, OnDestroy} from '@angular/core';
import {Group} from "./group.model";
import {GroupService} from "./group.service";
import {GroupActionService} from "./group-action.service";
import {CharacterService} from "../character/character.service";
import {NotificationsService} from "../notifications/notifications.service";
import {Loot} from "./loot.model";
import {Item} from "../character/item.model";
import {ItemService} from "../item/item.service";
import {LootWebsocketService} from "./loot.websocket.service";
import {WsEvent} from "../shared/websocket.model";
import {Monster} from "../monster/monster.model";

@Component({
    selector: 'loot-panel',
    templateUrl: 'loot-panel.component.html',
    providers: [LootWebsocketService],
})
export class LootPanelComponent implements OnInit, OnDestroy {
    @Input() group: Group;
    public loots: Loot[] = [];
    public lootTargetForNewItem: Loot;
    public newLootName: string;

    constructor(private _groupService: GroupService
        , private _actionService: GroupActionService
        , private _itemService: ItemService
        , private _lootWebsocketService: LootWebsocketService
        , private _characterService: CharacterService
        , private _notification: NotificationsService) {
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

    deleteLoot(loot: Loot) {
        this._groupService.deleteLoot(loot.id).subscribe(
            loot => {
                this.onDeleteLoot(loot);
            }
        );
    }

    addItemToLoot(loot: Loot, item: Item) {
        if (item != null) {
            this._itemService.addItemTo('loot', loot.id, item.template.id, item.data).subscribe(
                item => {
                    LootPanelComponent.onAddItemToLoot(loot, item);
                }
            );
        }
        this.lootTargetForNewItem = null;
    }

    removeItemFromLoot(loot: Loot, item: Item) {
        if (item != null) {
            this._itemService.deleteItem(item.id).subscribe(
                item => {
                    LootPanelComponent.onRemoveItemFromLoot(loot, item);
                }
            );
        }
        this.lootTargetForNewItem = null;
    }

    removeItemFromMonster(monster: Monster, item: Item) {
        if (item != null) {
            this._itemService.deleteItem(item.id).subscribe(
                item => {
                    LootPanelComponent.onRemoveItemFromMonster(monster, item);
                }
            );
        }
        this.lootTargetForNewItem = null;
    }

    getLootById(id: number): Loot {
        for (let i = 0; i < this.loots.length; i++) {
            let loot = this.loots[i];
            if (loot.id == id) {
                return loot;
            }
        }
        return null;
    }

    static onAddItemToLoot(loot: Loot, data: any) {
        for (let i = 0; i < loot.items.length; i++) {
            if (loot.items[i].id == data.id) {
                return;
            }
        }
        loot.items.push(data);
    }

    static onRemoveItemFromLoot(loot: Loot, data: any) {
        for (let i = 0; i < loot.items.length; i++) {
            if (loot.items[i].id == data.id) {
                loot.items.splice(i, 1);
            }
        }
    }

    static onRemoveItemFromMonster(monster: Monster, data: any) {
        for (let i = 0; i < monster.items.length; i++) {
            if (monster.items[i].id == data.id) {
                monster.items.splice(i, 1);
            }
        }
    }

    static onAddMonsterToLoot(loot: Loot, monster: Monster) {
        for (let i = 0; i < loot.monsters.length; i++) {
            if (loot.monsters[i].id == monster.id) {
                return;
            }
        }
        loot.monsters.push(monster);
    }


    static onRemoveMonsterFromLoot(loot: Loot, monster: Monster) {
        for (let i = 0; i < loot.monsters.length; i++) {
            if (loot.monsters[i].id == monster.id) {
                loot.monsters.splice(i, 1);
            }
        }
    }

    notifyChange(message: string) {
        this._notification.info("Modification", message);
    }

    wrapLootResult(callback: Function): Function {
        return r => {
            let loot = this.getLootById(r.id);
            if (loot) {
                callback(loot, r.data);
            }
        }
    }

    onAddLoot(loot: Loot) {
        let lootIdx = this.loots.findIndex(l => l.id == loot.id);
        if (lootIdx != -1) {
            return;
        }
        this.loots.unshift(loot);
    }

    onDeleteLoot(loot: Loot) {
        let lootIdx = this.loots.findIndex(l => l.id == loot.id);
        if (lootIdx != -1) {
            this.loots.splice(lootIdx, 1);
        }
    }

    registerWs() {
        for (let i = 0; i < this.loots.length; i++) {
            this._lootWebsocketService.register(this.loots[i].id);
        }
        this._lootWebsocketService.registerNotifyFunction(this.notifyChange.bind(this));
        this._lootWebsocketService.registerPacket("addItem").subscribe(this.wrapLootResult(LootPanelComponent.onAddItemToLoot).bind(this));
        this._lootWebsocketService.registerPacket("deleteItem").subscribe(this.wrapLootResult(LootPanelComponent.onRemoveItemFromLoot).bind(this));
        this._lootWebsocketService.registerPacket("addMonster").subscribe(this.wrapLootResult(LootPanelComponent.onAddMonsterToLoot).bind(this));
        this._lootWebsocketService.registerPacket("deleteMonster").subscribe(this.wrapLootResult(LootPanelComponent.onRemoveMonsterFromLoot).bind(this));
    }

    registerActions() {
        this._actionService.registerAction("addLoot").map(a => a.data).subscribe(this.onAddLoot.bind(this));
        this._actionService.registerAction("deleteLoot").map(a => a.data).subscribe(this.onDeleteLoot.bind(this));
    }

    ngOnDestroy(): void {
        for (let i = 0; i < this.loots.length; i++) {
            this._lootWebsocketService.unregister(this.loots[i].id);
        }
    }

    ngOnInit(): void {
        this._groupService.loadLoots(this.group.id).subscribe(
            loots => {
                this.loots = loots;
                this.registerWs();
                this.registerActions();
            }
        );
    }
}
