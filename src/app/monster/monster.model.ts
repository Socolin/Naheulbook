export class Monster {
    id: number;
    name: string;
    at: number;
    prd: number;
    ev: number;
    ea: number;
    pr: string;
    dmg: string;
    cou: number;
    resm: number;
    classeXP: number;
    note: string;
    dead: string;
    chercheNoise: boolean;
    target: {
        id: number;
        isMonster: number;
        name: string;
        color: string;
    };
    color: string;
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

