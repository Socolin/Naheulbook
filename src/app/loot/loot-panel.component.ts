import {OnDestroy} from '@angular/core';
import {NotificationsService} from "../notifications/notifications.service";
import {Loot} from "./loot.model";
import {LootWebsocketService} from "./loot.websocket.service";
import {Monster} from "../monster/monster.model";

export class LootPanelComponent implements OnDestroy {
    public loots: Loot[] = [];

    constructor(protected _lootWebsocketService: LootWebsocketService
        , protected _notification: NotificationsService) {
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

    static onRemoveItemFromMonsterLoot(loot: Loot, data: any) {
        let monsterIndex = loot.monsters.findIndex(m => m.id === data.monster.id);
        if (monsterIndex == -1) {
            return;
        }
        let monster = loot.monsters[monsterIndex];
        let itemIndex = monster.items.findIndex(i => data.item.id === i.id);
        if (itemIndex == -1) {
            return;
        }
        monster.items.splice(itemIndex, 1);
    }

    onTookItemFromLoot(loot: Loot, data: any) {
        let character = data.character;
        let itemIndex = loot.items.findIndex(i => data.item.id === i.id);
        if (itemIndex == -1) {
            return;
        }
        loot.items.splice(itemIndex, 1);
        this._notification.info("Loot", character.name + " à pris: " + data.item.name);
    }

    onTookItemFromMonsterLoot(loot: Loot, data: any) {
        let character = data.character;
        let monsterIndex = loot.monsters.findIndex(m => m.id === data.monster.id);
        if (monsterIndex == -1) {
            return;
        }
        let monster = loot.monsters[monsterIndex];
        let itemIndex = monster.items.findIndex(i => data.item.id === i.id);
        if (itemIndex == -1) {
            return;
        }
        monster.items.splice(itemIndex, 1);
        this._notification.info("Loot", character.name + " à pris: " + data.item.name);
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
        this._lootWebsocketService.register(loot.id);
        this.loots.unshift(loot);
    }

    onDeleteLoot(loot: Loot) {
        let lootIdx = this.loots.findIndex(l => l.id == loot.id);
        if (lootIdx != -1) {
            this.loots.splice(lootIdx, 1);
        }
        this._lootWebsocketService.unregister(loot.id);
    }

    onUpdateLoot(loot: Loot) {
        let lootIdx = this.loots.findIndex(l => l.id == loot.id);
        if (lootIdx != -1) {
            this.loots[lootIdx].visibleForPlayer = loot.visibleForPlayer;
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
        this._lootWebsocketService.registerPacket("deleteItemMonster").subscribe(this.wrapLootResult(LootPanelComponent.onRemoveItemFromMonsterLoot).bind(this));
        this._lootWebsocketService.registerPacket("tookItem").subscribe(this.wrapLootResult(this.onTookItemFromLoot.bind(this)).bind(this));
        this._lootWebsocketService.registerPacket("tookItemMonster").subscribe(this.wrapLootResult(this.onTookItemFromMonsterLoot.bind(this)).bind(this));
    }

    ngOnDestroy(): void {
        for (let i = 0; i < this.loots.length; i++) {
            this._lootWebsocketService.unregister(this.loots[i].id);
        }
    }

    onLoadLoots(loots: Loot[]) {
        this.loots = loots;
        this.registerWs();
    }
}
