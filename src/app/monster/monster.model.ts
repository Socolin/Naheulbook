import {ItemTemplate} from '../item/item-template.model';
import {Item} from '../character/item.model';

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
