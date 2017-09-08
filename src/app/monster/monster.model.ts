import {ItemTemplate} from '../item/item-template.model';
import {Item, PartialItem} from '../character/item.model';
import {Subject} from 'rxjs/Subject';
import {WsRegistrable} from '../websocket/websocket.model';
import {TargetJsonData} from '../group/target.model';
import {WebSocketService} from '../websocket/websocket.service';
import {Skill} from '../skill/skill.model';
import {ActiveStatsModifier, StatModifier} from '../shared/stat-modifier.model';
import {isNullOrUndefined} from 'util';
import {Fighter} from '../group/group.model';

export class MonsterData {
    at: number;
    prd: number | undefined;
    esq: number | undefined;
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
    color = '000000';
    number: number;
    sex: string;
    page: number;

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


export class MonsterComputedData {
    at: number;
    prd: number;
    esq: number;
    pr: number;
    pr_magic: number;
    dmg: {name: string, damage: string, incompatible?: boolean}[] = [];
    cou: number;
    chercheNoise: boolean;
    resm: number;
}

export class Monster extends WsRegistrable {
    public id: number;
    public name: string;
    public data: MonsterData = new MonsterData();
    public dead: string;
    public items: Item[];

    public modifiers: ActiveStatsModifier[] = [];

    public itemAdded: Subject<Item> = new Subject<Item>();
    public itemRemoved: Subject<Item> = new Subject<Item>();
    public targetChanged: Subject<TargetJsonData> = new Subject<TargetJsonData>();

    public target: TargetJsonData;

    public computedData: MonsterComputedData = new MonsterComputedData();
    public onChange: Subject<any> = new Subject<any>();
    public onNotification: Subject<any> = new Subject<any>();

    static fromJson(jsonData: any, skillsById: {[skillId: number]: Skill}): Monster {
        let monster = new Monster();
        Object.assign(monster, jsonData, {
            data: MonsterData.fromJson(jsonData.data),
            items: Item.itemsFromJson(jsonData.items, skillsById),
            modifiers: ActiveStatsModifier.modifiersFromJson(jsonData.modifiers)
        });
        monster.update();
        return monster;
    }

    static monstersFromJson(monstersData: any[], skillsById: {[skillId: number]: Skill}): Monster[] {
        let monsters: Monster[] = [];
        for (let monsterData of monstersData) {
            monsters.push(Monster.fromJson(monsterData, skillsById));
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

    public takeItem(leftItem: PartialItem | Item | undefined, takenItem: PartialItem, character: object) {
        if (leftItem) {
            let currentItem = this.getItem(leftItem.id);
            if (currentItem) {
                currentItem.data.quantity = leftItem.data.quantity;
            }
        }
        else {
            const currentItem = this.getItem(takenItem.id);
            if (currentItem) {
                let i = this.items.findIndex(item => item.id === currentItem.id);
                if (i !== -1) {
                    let removedItem = this.items[i];
                    this.items.splice(i, 1);
                    this.itemRemoved.next(removedItem);
                }
            }
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
        this.onChange.next({action: 'changeData', fieldName: fieldName, value: value});
        this.update();
    }

    onAddModifier(modifier: ActiveStatsModifier) {
        for (let i = 0 ; i < this.modifiers.length; i++) {
            if (this.modifiers[i].id === modifier.id) {
                return;
            }
        }
        this.modifiers.push(modifier);
        this.update();
        this.notify('addModifier' , 'Ajout du modificateur: ' + modifier.name);
    }

    onRemoveModifier(modifier: ActiveStatsModifier) {
        for (let i = 0; i < this.modifiers.length; i++) {
            let e = this.modifiers[i];
            if (e. id === modifier.id) {
                this.modifiers.splice(i, 1);
                this.update();
                this.notify('removeModifier', 'Suppression du modificateur: ' + modifier.name);
                return;
            }
        }
    }

    onUpdateModifier(modifier: ActiveStatsModifier) {
        for (let i = 0; i < this.modifiers.length; i++) {
            if (this.modifiers[i].id === modifier.id) {
                if (this.modifiers[i].active === modifier.active
                    && this.modifiers[i].currentTimeDuration === modifier.currentTimeDuration
                    && this.modifiers[i].currentLapCount === modifier.currentLapCount
                    && this.modifiers[i].currentCombatCount === modifier.currentCombatCount) {
                    return;
                }
                if (!this.modifiers[i].active && modifier.active) {
                    this.notify('updateModifier', 'Activation du modificateur: ' + modifier.name);
                } else if (this.modifiers[i].active && !modifier.active) {
                    this.notify('updateModifier', 'Désactivation du modificateur: ' + modifier.name);
                } else {
                    this.notify('updateModifier', 'Mis à jour du modificateur: ' + modifier.name);
                }
                this.modifiers[i] .active = modifier.active;
                this.modifiers[i].currentCombatCount = modifier.currentCombatCount;
                this.modifiers[i] .currentTimeDuration = modifier.currentTimeDuration;
                this.modifiers[i].currentLapCount = modifier.currentLapCount;
                break;
            }
        }
        this.update();
    }

    private applyStatModifier(mod: StatModifier) {
        if (mod.stat === 'AT') {
            this.computedData.at = StatModifier.apply(this.computedData.at, mod);
        }
        if (mod.stat === 'PRD' && !isNullOrUndefined(this.computedData.prd)) {
            this.computedData.prd = StatModifier.apply(this.computedData.prd, mod);
        }
        if (mod.stat === 'AD') {
            this.computedData.esq = StatModifier.apply(this.computedData.esq, mod);
        }
        if (mod.stat === 'ESQ') {
            this.computedData.esq = StatModifier.apply(this.computedData.esq, mod);
        }
        if (mod.stat === 'PR') {
            this.computedData.pr = StatModifier.apply(this.computedData.pr, mod);
        }
        if (mod.stat === 'PR_MAGIC') {
            this.computedData.pr_magic = StatModifier.apply(this.computedData.pr_magic, mod);
        }
        if (mod.stat === 'RESM') {
            this.computedData.cou = StatModifier.apply(this.computedData.resm, mod);
        }
    }

    equipItem(partialItem: PartialItem) {
        let item = this.getItem(partialItem.id)
        if (!item) {
            return;
        }
        if (item.data.equiped === partialItem.data.equiped) {
            return;
        }
        item.data.equiped = partialItem.data.equiped;
        this.update();
    }

    update() {
        this.computedData.at =  this.data.at;
        this.computedData.prd =  this.data.prd ? this.data.prd : 0;
        this.computedData.esq =  this.data.esq ? this.data.esq : 0;
        this.computedData.pr =  this.data.pr;
        this.computedData.pr_magic =  this.data.pr_magic;
        this.computedData.dmg =  [{name: 'base', damage: this.data.dmg}];
        this.computedData.cou =  this.data.cou;
        this.computedData.chercheNoise =  this.data.chercheNoise;
        this.computedData.resm =  this.data.resm;

        for (let activeModifier of this.modifiers) {
            if (activeModifier.active) {
                for (let mod of activeModifier.values) {
                    this.applyStatModifier(mod);
                }
            }
        }
        for (let item of this.items) {
            if (item.data.equiped) {
                this.computedData.dmg.push({
                    name: item.data.name,
                    damage: item.getDamageString()
                });
            }
            if (item.data.equiped && item.template.modifiers) {
                for (let mod of item.template.modifiers) {
                    this.applyStatModifier(mod);
                }
            }
        }
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

    public updateTime(type: string, data: number | { previous: Fighter; next: Fighter }): any[] {
        let changes: any[] = [];
        for (let item of this.items) {
            for (let i = 0; i < item.modifiers.length; i++) {
                let modifier = item.modifiers[i];
                if (modifier.updateDuration(type, data)) {
                    changes.push({type: 'item', itemId: item.id, modifierIdx: i, modifier: modifier});
                }
            }
        }
        for (let modifier of this.modifiers) {
            if (modifier.updateDuration(type, data)) {
                changes.push({type: 'modifier', modifier: modifier});
            }
        }
        return changes;
    }

    updateLapDecrement(data: { deleted: Fighter; previous: Fighter; next: Fighter }): any[] {
        let changes: any[] = [];
        for (let item of this.items) {
            for (let i = 0; i < item.modifiers.length; i++) {
                let modifier = item.modifiers[i];
                if (modifier.updateLapDecrement(data)) {
                    changes.push({type: 'item', itemId: item.id, modifierIdx: i, modifier: modifier});
                }
            }
        }
        for (let modifier of this.modifiers) {
            if (modifier.updateLapDecrement(data)) {
                changes.push({type: 'modifier', modifier: modifier});
            }
        }
        return changes;
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
    prd: number | undefined;
    esq: number | undefined;
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
    page: number;
}

export class MonsterTemplateCategory {
    id: number;
    name: string;
    type: MonsterTemplateType;

    static fromJson(jsonData: any, type: {[id: number]: MonsterTemplateType}|MonsterTemplateType): MonsterTemplateCategory {
        let category = new MonsterTemplateCategory();
        if (type instanceof  MonsterTemplateType) {
            Object.assign(category, jsonData, {type: type});
        } else {
            Object.assign(category, jsonData, {type: type[jsonData.typeId]});
        }
        return category;
    }

    static categoriesFromJson(jsonDatas: any[], type: MonsterTemplateType): MonsterTemplateCategory[] {
        let categories: MonsterTemplateCategory[] = [];

        for (let jsonData of jsonDatas) {
            categories.push(MonsterTemplateCategory.fromJson(jsonData, type));
        }

        return categories;
    }
}

export class MonsterTemplateType {
    id: number;
    name: string;
    categories: MonsterTemplateCategory[] = [];

    static fromJson(jsonData: any): MonsterTemplateType {
        let type = new MonsterTemplateType();
        Object.assign(type, jsonData, {categories: MonsterTemplateCategory.categoriesFromJson(jsonData.categories, type)});
        return type;
    }

    static typesFromJson(jsonDatas: any[]): MonsterTemplateType[] {
        let types: MonsterTemplateType[] = [];
        for (let jsonData of jsonDatas) {
            types.push(MonsterTemplateType.fromJson(jsonData));
        }
        return types;
    }
}

export class MonsterSimpleInventory {
    id: number;
    itemTemplate: ItemTemplate;
    minCount: number;
    maxCount: number;
    chance: number;
    equiped: boolean;
    hidden: boolean;
}

export class MonsterTemplate {
    id: number;
    name: string;
    data: MonsterTemplateData;
    categoryId: number;
    category: MonsterTemplateCategory;
    simpleInventory: MonsterSimpleInventory[];
    locations: number[];

    static fromJson(jsonData: any, categoriesById: {[id: number]: MonsterTemplateCategory}): MonsterTemplate {
        let monsterTemplate = new MonsterTemplate();
        Object.assign(monsterTemplate, jsonData, {category: categoriesById[jsonData.categoryId]});
        return monsterTemplate;
    }

    static templatessFromJson(jsonDatas: any[], categoriesById: {[id: number]: MonsterTemplateCategory}): MonsterTemplate[] {
        let templates: MonsterTemplate[] = [];
        for (let jsonData of jsonDatas) {
            templates.push(MonsterTemplate.fromJson(jsonData, categoriesById));
        }
        return templates;
    }
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
