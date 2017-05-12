import {OnDestroy} from '@angular/core';
import {NotificationsService} from '../notifications/notifications.service';
import {Loot, LootTookItemMsg} from './loot.model';
import {LootWebsocketService} from './loot.websocket.service';
import {Monster} from '../monster/monster.model';
import {Subscription} from 'rxjs';
import {Item} from '../character/item.model';

export class LootPanelComponent implements OnDestroy {
    public loots: Loot[] = [];
    public subscriptions: Subscription[] = [];

    constructor(protected _lootWebsocketService: LootWebsocketService
        , protected _notification: NotificationsService) {
    }

    getLootById(id: number): Loot {
        for (let i = 0; i < this.loots.length; i++) {
            let loot = this.loots[i];
            if (loot.id === id) {
                return loot;
            }
        }
        return null;
    }

    onAddItemToLoot(loot: Loot, data: any) {
        for (let i = 0; i < loot.items.length; i++) {
            if (loot.items[i].id === data.id) {
                return;
            }
        }
        loot.items.push(data);
    }

    onAddItemToMonster(loot: Loot, data: any) {
        let monsterIndex = loot.monsters.findIndex(m => m.id === data.monster.id);
        if (monsterIndex === -1) {
            return;
        }
        let monster = loot.monsters[monsterIndex];
        let itemIndex = monster.items.findIndex(i => data.item.id === i.id);
        if (itemIndex !== -1) {
            return;
        }
        monster.items.push(data.item);
    }

    onRemoveItemFromLoot(loot: Loot, data: Item) {
        for (let i = 0; i < loot.items.length; i++) {
            if (loot.items[i].id === data.id) {
                loot.items.splice(i, 1);
                break;
            }
        }
    }

    onRemoveItemFromMonster(monster: Monster, data: Item) {
        for (let i = 0; i < monster.items.length; i++) {
            if (monster.items[i].id === data.id) {
                monster.items.splice(i, 1);
                break;
            }
        }
    }

    onAddMonsterToLoot(loot: Loot, monster: Monster) {
        for (let i = 0; i < loot.monsters.length; i++) {
            if (loot.monsters[i].id === monster.id) {
                return;
            }
        }
        loot.monsters.push(monster);
        this.updateLootXp(loot);
    }


    onRemoveMonsterFromLoot(loot: Loot, monster: Monster) {
        for (let i = 0; i < loot.monsters.length; i++) {
            if (loot.monsters[i].id === monster.id) {
                loot.monsters.splice(i, 1);
            }
        }
        this.updateLootXp(loot);
    }

    onRemoveItemFromMonsterLoot(loot: Loot, data: any) {
        let monsterIndex = loot.monsters.findIndex(m => m.id === data.monster.id);
        if (monsterIndex === -1) {
            return;
        }
        let monster = loot.monsters[monsterIndex];
        let itemIndex = monster.items.findIndex(i => data.item.id === i.id);
        if (itemIndex === -1) {
            return;
        }
        monster.items.splice(itemIndex, 1);
    }

    onTookItemFromLoot(loot: Loot, data: LootTookItemMsg) {
        let character = data.character;
        let lootItemId = data.leftItem ? data.leftItem.id : data.item.id;
        let itemIndex = loot.items.findIndex(i => lootItemId === i.id);
        if (itemIndex === -1) {
            return;
        }
        if (data.leftItem) {
            let newQuantity = data.leftItem.data.quantity;
            if (loot.items[itemIndex].data.quantity === newQuantity) {
                return;
            }
            loot.items[itemIndex].data.quantity = newQuantity;
            this._lootWebsocketService.notifyChange(character.name + ' à pris: ' + data.quantity + ' ' + data.item.data.name);
        }
        else {
            loot.items.splice(itemIndex, 1);
            if (data.quantity) {
                this._lootWebsocketService.notifyChange(character.name + ' à pris: ' + data.quantity + ' ' + data.item.data.name);
            } else {
                this._lootWebsocketService.notifyChange(character.name + ' à pris: ' + data.item.data.name);
            }
        }
    }

    onTookItemFromMonsterLoot(loot: Loot, data: LootTookItemMsg) {
        let character = data.character;
        let monsterIndex = loot.monsters.findIndex(m => m.id === data.monster.id);
        if (monsterIndex === -1) {
            return;
        }
        let monster = loot.monsters[monsterIndex];
        let lootItemId = data.leftItem ? data.leftItem.id : data.item.id;
        let itemIndex = monster.items.findIndex(i => lootItemId === i.id);
        if (itemIndex === -1) {
            return;
        }
        if (data.leftItem) {
            let newQuantity = data.leftItem.data.quantity;
            if (monster.items[itemIndex].data.quantity === newQuantity) {
                return;
            }
            monster.items[itemIndex].data.quantity = newQuantity;
            this._lootWebsocketService.notifyChange(character.name + ' à pris: ' + data.quantity + ' ' + data.item.data.name);
        }
        else {
            monster.items.splice(itemIndex, 1);
            if (data.quantity) {
                this._lootWebsocketService.notifyChange(character.name + ' à pris: ' + data.quantity + ' ' + data.item.data.name);
            } else {
                this._lootWebsocketService.notifyChange(character.name + ' à pris: ' + data.item.data.name);
            }
        }
    }

    notifyChange(message: string) {
        this._notification.info('Loot', message);
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
        let lootIdx = this.loots.findIndex(l => l.id === loot.id);
        if (lootIdx !== -1) {
            return;
        }
        this._lootWebsocketService.register(loot.id);
        this.loots.unshift(loot);
        this.updateLootXp(loot);
    }

    onDeleteLoot(loot: Loot) {
        let lootIdx = this.loots.findIndex(l => l.id === loot.id);
        if (lootIdx !== -1) {
            this.loots.splice(lootIdx, 1);
        }
        this._lootWebsocketService.unregister(loot.id);
        this.updateLootXp(loot);
    }

    onUpdateLoot(loot: Loot) {
        let lootIdx = this.loots.findIndex(l => l.id === loot.id);
        if (lootIdx !== -1) {
            this.loots[lootIdx].visibleForPlayer = loot.visibleForPlayer;
        }
    }

    registerWs() {
        for (let i = 0; i < this.loots.length; i++) {
            this._lootWebsocketService.register(this.loots[i].id);
        }
        this._lootWebsocketService.registerNotifyFunction(this.notifyChange.bind(this));

        this.subscriptions.push(this._lootWebsocketService.registerPacket('addItem')
            .subscribe(this.wrapLootResult(this.onAddItemToLoot.bind(this)).bind(this)));
        this.subscriptions.push(this._lootWebsocketService.registerPacket('addItemMonster')
            .subscribe(this.wrapLootResult(this.onAddItemToMonster.bind(this)).bind(this)));
        this.subscriptions.push(this._lootWebsocketService.registerPacket('deleteItem')
            .subscribe(this.wrapLootResult(this.onRemoveItemFromLoot.bind(this)).bind(this)));
        this.subscriptions.push(this._lootWebsocketService.registerPacket('addMonster')
            .subscribe(this.wrapLootResult(this.onAddMonsterToLoot.bind(this)).bind(this)));
        this.subscriptions.push(this._lootWebsocketService.registerPacket('deleteMonster')
            .subscribe(this.wrapLootResult(this.onRemoveMonsterFromLoot.bind(this)).bind(this)));
        this.subscriptions.push(this._lootWebsocketService.registerPacket('deleteItemMonster')
            .subscribe(this.wrapLootResult(this.onRemoveItemFromMonsterLoot.bind(this)).bind(this)));
        this.subscriptions.push(this._lootWebsocketService.registerPacket('tookItem')
            .subscribe(this.wrapLootResult(this.onTookItemFromLoot.bind(this)).bind(this)));
        this.subscriptions.push(this._lootWebsocketService.registerPacket('tookItemMonster')
            .subscribe(this.wrapLootResult(this.onTookItemFromMonsterLoot.bind(this)).bind(this)));
    }

    updateXp() {
        for (let i = 0; i < this.loots.length; i++) {
            this.updateLootXp(this.loots[i]);
        }
    }

    updateLootXp(loot: Loot) {
        let xp = 0;
        for (let j = 0; j < loot.monsters.length; j++) {
            let monster = loot.monsters[j];
            if (monster.data.xp) {
                xp += monster.data.xp;
            }
        }
        loot.computedXp = xp;
    }

    ngOnDestroy(): void {
        for (let i = 0; i < this.loots.length; i++) {
            this._lootWebsocketService.unregister(this.loots[i].id);
        }
        for (let i = 0; i < this.subscriptions.length; i++) {
            this.subscriptions[i].unsubscribe();
        }
    }

    onLoadLoots(loots: Loot[]) {
        this.loots = loots;
        this.updateXp();
        this.registerWs();
    }
}
