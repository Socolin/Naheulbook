import {Subject} from 'rxjs';
import {isNullOrUndefined} from 'util';

import {ActiveStatsModifier, DurationChange, IMetadata, StatModifier} from '../shared';
import {Skill, SkillDictionary} from '../skill';
import {ItemTemplate} from '../item-template';
import {Item, PartialItem} from '../item';
import {WebSocketService, WsEventServices, WsRegistrable} from '../websocket';

import {TargetJsonData} from '../group/target.model';
import {Fighter} from '../group';
import {MonsterCategoryResponse, MonsterResponse, MonsterTemplateResponse, MonsterTypeResponse} from '../api/responses';
import {MonsterTemplateData} from '../api/shared';

export class MonsterData {
    at: number;
    prd?: number;
    esq?: number;
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
    sex?: string;
    page?: number;

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
    dmg: { name: string, damage: string, incompatible?: boolean }[] = [];
    cou: number;
    chercheNoise: boolean;
    resm: number;
}

export class Monster extends WsRegistrable {
    public id: number;
    public name: string;
    public data: MonsterData = new MonsterData();
    public dead: string;
    public items: Item[] = [];

    public modifiers: ActiveStatsModifier[] = [];

    public itemAdded: Subject<Item> = new Subject<Item>();
    public itemRemoved: Subject<Item> = new Subject<Item>();
    public targetChanged: Subject<TargetJsonData> = new Subject<TargetJsonData>();

    public target: TargetJsonData;

    public computedData: MonsterComputedData = new MonsterComputedData();
    public onChange: Subject<any> = new Subject<any>();
    public onNotification: Subject<any> = new Subject<any>();

    static fromJson(jsonData: MonsterResponse, skillsById: { [skillId: number]: Skill }): Monster {
        let monster = new Monster();
        Object.assign(monster, jsonData, {
            data: MonsterData.fromJson(jsonData.data),
            items: Item.itemsFromJson(jsonData.items, skillsById),
            modifiers: ActiveStatsModifier.modifiersFromJson(jsonData.modifiers)
        });
        monster.update();
        return monster;
    }

    static monstersFromJson(monstersData: any[] | undefined, skillsById: { [skillId: number]: Skill }): Monster[] {
        let monsters: Monster[] = [];

        if (monstersData) {
            for (let monsterData of monstersData) {
                monsters.push(Monster.fromJson(monsterData, skillsById));
            }
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

    public takeItem(itemId: number, remainingQuantity: number | undefined, character: IMetadata) {
        if (remainingQuantity) {
            const item = this.getItem(itemId);
            if (item) {
                item.data.quantity = remainingQuantity;
                this.onChange.next({action: 'tookItem', character: character, item: item});
            }
        } else {
            const item = this.getItem(itemId);
            if (item) {
                let i = this.items.findIndex(it => it.id === itemId);
                if (i !== -1) {
                    let removedItem = this.items[i];
                    this.items.splice(i, 1);
                    this.itemRemoved.next(removedItem);
                }
                this.onChange.next({action: 'tookItem', character: character, item: item});
            }
        }
    }

    changeTarget(target: TargetJsonData) {
        this.target = target;
        this.targetChanged.next(target);
    }

    public changeData(data: MonsterData) {
        for (let fieldName in data) {
            if (!data.hasOwnProperty(fieldName)) {
                continue;
            }
            if (data[fieldName] !== this.data[fieldName]) {
                this.notify('changeData',
                    'Modification: ' + fieldName.toUpperCase() + ': ' + this.data[fieldName] + ' -> ' + data[fieldName],
                    {fieldName: fieldName, value: data[fieldName]});
                this.onChange.next({action: 'changeData', fieldName: fieldName, value: data[fieldName]});
            }
        }
        this.data = data;
        this.update();
    }

    onAddModifier(modifier: ActiveStatsModifier) {
        for (let i = 0; i < this.modifiers.length; i++) {
            if (this.modifiers[i].id === modifier.id) {
                return;
            }
        }
        this.modifiers.push(modifier);
        this.update();
        this.notify('addModifier', 'Ajout du modificateur: ' + modifier.name);
    }

    onRemoveModifier(modifierId: number) {
        for (let i = 0; i < this.modifiers.length; i++) {
            let e = this.modifiers[i];
            if (e.id === modifierId) {
                this.modifiers.splice(i, 1);
                this.update();
                this.notify('removeModifier', 'Suppression du modificateur: ' + e.name);
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
                this.modifiers[i].active = modifier.active;
                this.modifiers[i].currentCombatCount = modifier.currentCombatCount;
                this.modifiers[i].currentTimeDuration = modifier.currentTimeDuration;
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
        let item = this.getItem(partialItem.id);
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
        this.computedData.at = this.data.at;
        this.computedData.prd = this.data.prd ? this.data.prd : 0;
        this.computedData.esq = this.data.esq ? this.data.esq : 0;
        this.computedData.pr = this.data.pr;
        this.computedData.pr_magic = this.data.pr_magic;
        if (!this.computedData.pr_magic) {
            this.computedData.pr_magic = 0;
        }
        this.computedData.dmg = [{name: 'base', damage: this.data.dmg}];
        this.computedData.cou = this.data.cou;
        this.computedData.chercheNoise = this.data.chercheNoise;
        this.computedData.resm = this.data.resm;

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
                    name: item.data.name || item.template.name,
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

    updateLapDecrement(data: { deleted: Fighter; previous: Fighter; next: Fighter }): DurationChange[] {
        let changes: DurationChange[] = [];
        for (let item of this.items) {
            for (let i = 0; i < item.modifiers.length; i++) {
                let modifier = item.modifiers[i];
                if (modifier.updateLapDecrement(data)) {
                    changes.push({type: 'itemModifier', itemId: item.id, modifier: modifier});
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


    handleWebsocketEvent(opcode: string, data: any, services: WsEventServices) {
        switch (opcode) {
            case 'addItem': {
                services.skill.getSkillsById().subscribe(skillsById => {
                    this.addItem(Item.fromJson(data, skillsById));
                });
                break;
            }
            case 'deleteItem': {
                this.removeItem(data);
                break;
            }
            case 'tookItem': {
                this.takeItem(data.originalItem.id, data.remainingQuantity, data.character);
                break;
            }
            case 'changeName': {
                this.name = data;
                break;
            }
            case 'changeTarget': {
                this.changeTarget(data);
                break;
            }
            case 'changeData': {
                this.changeData(MonsterData.fromJson(data));
                break;
            }
            case 'addModifier': {
                this.onAddModifier(ActiveStatsModifier.fromJson(data));
                break;
            }
            case 'removeModifier': {
                this.onRemoveModifier(data);
                break;
            }
            case 'updateModifier': {
                this.onUpdateModifier(ActiveStatsModifier.fromJson(data));
                break;
            }
            case 'equipItem': {
                this.equipItem(PartialItem.fromJson(data));
                break;
            }
            default: {
                console.warn('Opcode not handle: `' + opcode + '`');
                break;
            }
        }
    }
}

export class MonsterInventoryElement {
    id: number;
    itemTemplate: ItemTemplate;
    minCount: number;
    maxCount: number;
    chance: number;
    hidden: boolean;
    minUg?: number;
    maxUg?: number;
}

export class TraitInfo {
    traitId: number;
    level: number;

    constructor(id: number, level: number) {
        this.traitId = id;
        this.level = level;
    }
}

export type MonsterTemplateCategoryDictionary = { [id: number]: MonsterTemplateCategory };

export class MonsterTemplateCategory {
    id: number;
    name: string;
    type: MonsterTemplateType;

    static fromResponse(
        response: MonsterCategoryResponse,
        type: { [id: number]: MonsterTemplateType } | MonsterTemplateType
    ): MonsterTemplateCategory {
        let category = new MonsterTemplateCategory();
        if (type instanceof MonsterTemplateType) {
            Object.assign(category, response, {type: type});
        } else {
            Object.assign(category, response, {type: type[response.typeid]});
        }
        return category;
    }

    static categoriesFromJson(jsonDatas: MonsterCategoryResponse[], type: MonsterTemplateType): MonsterTemplateCategory[] {
        let categories: MonsterTemplateCategory[] = [];

        for (let jsonData of jsonDatas) {
            categories.push(MonsterTemplateCategory.fromResponse(jsonData, type));
        }

        return categories;
    }
}

export class MonsterTemplateType {
    id: number;
    name: string;
    categories: MonsterTemplateCategory[] = [];

    static fromResponse(response: MonsterTypeResponse): MonsterTemplateType {
        let type = new MonsterTemplateType();
        Object.assign(type, response, {categories: MonsterTemplateCategory.categoriesFromJson(response.categories, type)});
        return type;
    }

    static typesFromJson(jsonDatas: MonsterTypeResponse[]): MonsterTemplateType[] {
        let types: MonsterTemplateType[] = [];
        for (let jsonData of jsonDatas) {
            types.push(MonsterTemplateType.fromResponse(jsonData));
        }
        return types;
    }
}


export class MonsterTemplate {
    id: number;
    name: string;
    data: MonsterTemplateData;
    categoryId: number;
    category: MonsterTemplateCategory;
    simpleInventory: MonsterInventoryElement[];

    static fromResponse(
        response: MonsterTemplateResponse,
        categoriesById: MonsterTemplateCategoryDictionary,
        skillsById: SkillDictionary
    ): MonsterTemplate {
        const category = categoriesById[response.categoryId];
        const inventory: MonsterInventoryElement[] = [];
        for (let inventoryElement of response.simpleInventory) {
            let element = {
                ...inventoryElement,
                itemTemplate: ItemTemplate.fromResponse(inventoryElement.itemTemplate, skillsById)
            };
            inventory.push(element);
        }

        return new MonsterTemplate(response.id, response.name, category, response.data, inventory);
    }

    static templatesFromResponse(
        responses: MonsterTemplateResponse[],
        categoriesById: MonsterTemplateCategoryDictionary,
        skillsById: SkillDictionary
    ): MonsterTemplate[] {
        return responses.map(response => MonsterTemplate.fromResponse(response, categoriesById, skillsById));
    }

    constructor(
        id: number,
        name: string,
        category: MonsterTemplateCategory,
        data: MonsterTemplateData,
        inventory: MonsterInventoryElement[]
    ) {
        this.id = id;
        this.name = name;
        this.category = category;
        this.categoryId = category.id; // FIXME: still needed ?
        this.data = data;
        this.simpleInventory = inventory;
    }
}

export type MonsterTraitDictionary = { [id: number]: MonsterTrait };

export class MonsterTrait {
    id: number;
    name: string;
    description: string;
    levels?: string[];
}
