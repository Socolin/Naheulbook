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
}

export class Monster {
    id: number;
    name: string;
    data: MonsterData = new MonsterData();
    dead: string;
    target: {
        id: number;
        isMonster: number;
        name: string;
        color: string;
    };
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

