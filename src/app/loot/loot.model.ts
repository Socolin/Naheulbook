import {Subject, Subscription} from 'rxjs';

import {IMetadata} from '../shared';
import {WsRegistrable, WebSocketService, WsEventServices} from '../websocket';

import {SkillDictionary} from '../skill';
import {Item, ItemData, PartialItem} from '../item';
import {Monster} from '../monster';
import {ItemPartialResponse, LootResponse} from '../api/responses';

export class LootTookItemMsg {
    originalItem: PartialItem;
    character: IMetadata;
    remainingQuantity?: number;
}

export class Loot extends WsRegistrable {
    public id: number;
    public name: string;
    public visibleForPlayer: boolean;

    public items: Item[];
    public itemAdded: Subject<Item> = new Subject<Item>();
    public itemRemoved: Subject<Item> = new Subject<Item>();

    public monsters: Monster[];
    public monsterAdded: Subject<Monster> = new Subject<Monster>();
    public monsterRemoved: Subject<Monster> = new Subject<Monster>();
    public monsterSubscriptions: {[monsterId: number]: Subscription} = {};

    public onTookItem: Subject<any> = new Subject<any>();

    public computedXp: number;

    static fromResponse(response: LootResponse, skillsById: SkillDictionary): Loot {
        let loot = new Loot();
        Object.assign(loot, response, {
            monsters: Monster.monstersFromJson(response.monsters, skillsById),
            items: Item.itemsFromJson(response.items, skillsById)
        });
        loot.updateXp();
        return loot;
    }

    static lootsFromJson(responses: LootResponse[], skillsById: SkillDictionary): Loot[] {
        return responses.map(response => Loot.fromResponse(response, skillsById));
    }

    public getItem(itemId: number): Item | undefined {
        let i = this.items.findIndex(item => item.id === itemId);
        if (i !== -1) {
            return this.items[i];
        }
        return undefined;
    }

    /**
     * Add an item to the loot
     * @param addedItem The item to add
     * @returns {boolean} true if the item has been added (false if the item was already in)
     */
    public addItem(addedItem: Item): boolean {
        let i = this.items.findIndex(item => item.id === addedItem.id);
        if (i !== -1) {
            return false;
        }
        this.items.unshift(addedItem);
        this.itemAdded.next(addedItem);
        return true;
    }

    /**
     * Delete an item from the loot
     * @param removedItemId The id of the item to remove
     * @returns {boolean} true if item was removed (false if item was not present)
     */
    public removeItem(removedItemId: number): boolean {
        let i = this.items.findIndex(item => item.id === removedItemId);
        if (i !== -1) {
            let removedItem = this.items[i];
            this.items.splice(i, 1);
            this.itemRemoved.next(removedItem);
            return true;
        }
        return false;
    }

    /**
     * Add an monster to the loot
     * @param addedMonster The monster to add
     * @returns {boolean} true if the monster has been added (false if the monster was already in)
     */
    public addMonster(addedMonster: Monster): boolean {
        let i = this.monsters.findIndex(monster => monster.id === addedMonster.id);
        if (i !== -1) {
            return false;
        }
        this.monsters.push(addedMonster);
        this.updateXp();
        this.monsterAdded.next(addedMonster);
        this.monsterSubscriptions[addedMonster.id] = addedMonster.onChange
            .subscribe(() => this.updateXp());
        if (this.wsSubscribtion) {
            this.wsSubscribtion.service.registerElement(addedMonster);
        }
        return true;
    }

    /**
     * Delete an monster from the loot
     * @param monsterId The id of the monster to remove
     * @returns {boolean} true if monster was removed (false if monster was not present)
     */
    public removeMonster(monsterId: number): boolean {
        let i = this.monsters.findIndex(monster => monster.id === monsterId);
        if (i !== -1) {
            let removedMonster = this.monsters[i];
            this.monsters.splice(i, 1);
            this.updateXp();
            this.monsterRemoved.next(removedMonster);
            this.monsterSubscriptions[removedMonster.id].unsubscribe();
            delete this.monsterSubscriptions[removedMonster.id];
            if (this.wsSubscribtion) {
                this.wsSubscribtion.service.unregisterElement(removedMonster);
            }
            return true;
        }
        return false;
    }

    public updateXp() {
        let xp = 0;
        for (let j = 0; j < this.monsters.length; j++) {
            let monster = this.monsters[j];
            if (monster.data.xp) {
                xp += monster.data.xp;
            }
        }
        this.computedXp = xp;
    }

    public takeItem(itemId: number, remainingQuantity: number | undefined, character: IMetadata) {
        if (remainingQuantity) {
            const item = this.getItem(itemId);
            if (item) {
                item.data.quantity = remainingQuantity;
                this.onTookItem.next({character: character, item: item});
            }
        }
        else {
            const item = this.getItem(itemId);
            if (item) {
                let i = this.items.findIndex(it => it.id === itemId);
                if (i !== -1) {
                    let removedItem = this.items[i];
                    this.items.splice(i, 1);
                    this.itemRemoved.next(removedItem);
                }
                this.onTookItem.next({character: character, item: item});
            }
        }
    }

    public getWsTypeName(): string {
        return 'loot';
    }

    public onWsRegister(service: WebSocketService) {
        for (let monster of this.monsters) {
            service.registerElement(monster);
        }
    }

    public onWsUnregister(): void {
        if (!this.wsSubscribtion) {
            return;
        }

        for (let monster of this.monsters) {
            this.wsSubscribtion.service.unregisterElement(monster);
        }
    }

    public handleWebsocketEvent(opcode: string, data: any, services: WsEventServices) {
        switch (opcode) {
            case 'addItem': {
                services.skill.getSkillsById().subscribe(skillsById => {
                    this.addItem(Item.fromResponse(data, skillsById));
                });
                break;
            }
            case 'deleteItem': {
                this.removeItem(data);
                break;
            }
            case 'addMonster': {
                services.skill.getSkillsById().subscribe(skillsById => {
                    let monster = Monster.fromJson(data, skillsById);
                    this.addMonster(monster);
                });
                break;
            }
            case 'deleteMonster': {
                this.removeMonster(data.id);
                break;
            }
            case 'updateVisibility': {
                this.visibleForPlayer = data;
                break;
            }
            case 'updateItem': {
                this.onUpdateItem(data);
                break;
            }
            case 'tookItem': {
                this.takeItem(data.originalItem.id, data.remainingQuantity, data.character);
                break;
            }
        }
    }

    onUpdateItem(partialItem: ItemPartialResponse): void {
        let currentItem = this.items.find(i => i.id === partialItem.id);
        if (!currentItem) {
            return;
        }

        currentItem.data = new ItemData(partialItem.data);
    }

    public dispose() {
        for (let id in this.monsterSubscriptions) {
            if (this.monsterSubscriptions.hasOwnProperty(id)) {
                this.monsterSubscriptions[id].unsubscribe();
            }
        }
    }
}
