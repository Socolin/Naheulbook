import {ItemTemplate} from "../item/item-template.model";
import {Item} from "../character/item.model";
class MonsterData {
    at: number;
    prd: number;
    esq: number;
    ev: number;
    maxEv: number;
    ea: number;
    maxEa: number;
    pr: string;
    dmg: string;
    cou: number;
    chercheNoise: boolean;
    resm: number;
    xp: number;
    note: string;
    color: string;
    number: number;

    constructor(monsterData?: MonsterData) {
        if (monsterData) {
            for (let key in monsterData) {
                this[key] = monsterData[key];
            }
        }
    }
}

export class Monster {
    id: number;
    name: string;
    data: MonsterData = new MonsterData();
    dead: string;
    items: Item[] = [];
    target: {
        id: number;
        isMonster: boolean;
    };

    viewInventory: boolean;

    constructor(monster?: Monster) {
        if (monster) {
            this.id = monster.id;
            this.name = monster.name;
            this.data = new MonsterData(monster.data);
            this.dead = monster.dead;
            if (monster.target) {
                this.target = {
                    id: monster.target.id,
                    isMonster: monster.target.isMonster
                };
            }
        }
    }
}
export class TraitInfo {
    constructor(id: number, level: number) {
        this.traitId = id;
        this.level = level;
    }
    traitId: number;
    level: number;
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
    pr: string;
    resm: number;
    xp: number;
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
    constructor() {
        this.data = new MonsterTemplateData();
        this.locations = [];
        this.simpleInventory = [];
    }

    id: number;
    name: string;
    data: MonsterTemplateData;
    type: MonsterTemplateCategory;
    simpleInventory: MonsterSimpleInventory[];
    locations: number[];
}

export interface MonsterTrait {
    id: number;
    name: string;
    description: string;
    levels?: string[];
}
