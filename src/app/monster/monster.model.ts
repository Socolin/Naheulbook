class MonsterData {
    at: number;
    prd: number;
    ev: number;
    ea: number;
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

export class MonsterTemplate {
    constructor() {
        this.data = new MonsterTemplateData();
    }

    id: number;
    name: string;
    data: MonsterTemplateData;
    type: MonsterTemplateCategory;
    locations: number[];
}

export interface MonsterTrait {
    id: number;
    name: string;
    description: string;
    levels?: string[];
}
