import {ItemTemplate} from '../item/item-template.model';
import {Item, PartialItem} from '../character/item.model';
import {Subject} from 'rxjs/Subject';
import {WsRegistrable} from '../websocket/websocket.model';
import {TargetJsonData} from '../group/target.model';
import {WebSocketService} from '../websocket/websocket.service';
import {Skill} from '../skill/skill.model';

export class MonsterData {
    at: number;
    prd: number;
    esq: number;
    ev: number;
    maxEv: number;
    ea: number;
    maxEa: number;
    pr: number;
    pr_magic: number;
    dmg: string;
    cou: number;
    chercheNoise: boolean;
    resm: number;
    xp: number;
    note: string;
    color: string;
    number: number;
    sex: string;

    static fromJson(jsonData: any): MonsterData {
        let monsterData = new MonsterData();
        Object.assign(monsterData, jsonData);
        return monsterData;
    }

    constructor(monsterData?: MonsterData) {
        if (monsterData) {
            Object.assign(this, monsterData);
        }
    }
}

export class Monster extends WsRegistrable {
    public id: number;
    public name: string;
    public data: MonsterData = new MonsterData();
    public dead: string;
    public items: Item[];
    public itemAdded: Subject<Item> = new Subject<Item>();
    public itemRemoved: Subject<Item> = new Subject<Item>();
    public targetChanged: Subject<TargetJsonData> = new Subject<TargetJsonData>();

    public target: TargetJsonData;

    public onChange: Subject<any> = new Subject<any>();
    public onNotification: Subject<any> = new Subject<any>();

    static fromJson(jsonData: any, skillsById: {[skillId: number]: Skill}): Monster {
        let monster = new Monster();
        Object.assign(monster, jsonData, {
            data: MonsterData.fromJson(jsonData.data),
            items: Item.itemsFromJson(jsonData.items, skillsById)
        });
        return monster;
    }

    static monstersFromJson(monstersData: any[], skillsById: {[skillId: number]: Skill}): Monster[] {
        let monsters: Monster[] = [];
        for (let i = 0; i < monstersData.length; i++) {
            monsters.push(Monster.fromJson(monstersData[i], skillsById));
        }
        return monsters;
    }

    constructor(monster?: Monster) {
        super();
        if (monster) {
            Object.assign(this, monster, {data: new MonsterData(monster.data)});
            if (monster.target) {
                this.target = Object.assign({}, monster.target);
            }
        }
    }

    public notify(type: string, message: string, data?: any) {
        this.onNotification.next({type: type, message: message, data: data});
    }

    public getItem(itemId: number): Item {
        let i = this.items.findIndex(item => item.id === itemId);
        if (i !== -1) {
            return this.items[i];
        }
        return null;
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
        this.items.push(addedItem);
        this.itemAdded.next(addedItem);
        this.onChange.next({action: 'addItem', item: addedItem});
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
            this.onChange.next({action: 'removeItem', item: removedItem});
            return true;
        }
        return false;
    }

    public takeItem(leftItem: Item, takenItem: PartialItem, character: object) {
        if (leftItem) {
            let currentItem = this.getItem(leftItem.id);
            currentItem.data.quantity = leftItem.data.quantity;
        }
        else {
            let currentItem = this.getItem(takenItem.id);
            let i = this.items.findIndex(item => item.id === currentItem.id);
            let removedItem = this.items[i];
            this.items.splice(i, 1);
            this.itemRemoved.next(removedItem);
        }
        this.onChange.next({action: 'tookItem', character: character, item: takenItem});
    }

    changeTarget(target: TargetJsonData) {
        this.target = target;
        this.targetChanged.next(target);
    }

    public changeData(fieldName: string, value: any) {
        this.notify('changeData',
            'Modification: ' + fieldName.toUpperCase() + ': ' + this.data[fieldName] + ' -> ' + value,
            {fieldName: fieldName, value: value});
        this.data[fieldName] = value;
    }

    public getWsTypeName(): string {
        return 'monster';
    }

    public onWsRegister(service: WebSocketService) {
    }

    public onWsUnregister(): void {
    }

    dispose() {
        this.itemAdded.unsubscribe();
        this.itemRemoved.unsubscribe();
        this.targetChanged.unsubscribe();
        this.onNotification.unsubscribe();
        this.onChange.unsubscribe();
    }
}

export class TraitInfo {
    traitId: number;
    level: number;

    constructor(id: number, level: number) {
        this.traitId = id;
        this.level = level;
    }
}
export class MonsterTemplateData {
    at: number;
    prd: number;
    esq: number;
    ev: number;
    ea: number;
    cou: number;
    dmg: string;
    note: string;
    pr: number;
    pr_magic: number;
    resm: number;
    xp: number;
    chercheNoise: boolean;
    special: boolean;
    traits: TraitInfo[];
}
export class MonsterTemplateCategory {
    difficulty: string;
    id: number;
    name: string;
}
export class MonsterSimpleInventory {
    id: number;
    itemTemplate: ItemTemplate;
    minCount: number;
    maxCount: number;
    chance: number;
}

export class MonsterTemplate {
    id: number;
    name: string;
    data: MonsterTemplateData;
    type: MonsterTemplateCategory;
    simpleInventory: MonsterSimpleInventory[];
    locations: number[];

    constructor() {
        this.data = new MonsterTemplateData();
        this.locations = [];
        this.simpleInventory = [];
    }
}

export class MonsterTrait {
    id: number;
    name: string;
    description: string;
    levels?: string[];
}
