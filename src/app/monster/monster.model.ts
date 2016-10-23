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

export interface MonsterTemplateData {
    at: number;
    cou: number;
    dmg: string;
    ev: number;
    note: string;
    pr: string;
    prd: number;
    resm: number;
    xp: number;
    special: boolean;
}
export interface MonsterTemplateCategory {
    difficulty: string;
    id: number;
    name: string;
}

export interface MonsterTemplate {
    id: number;
    name: string;
    data: MonsterTemplateData;
    type: MonsterTemplateCategory;
}
