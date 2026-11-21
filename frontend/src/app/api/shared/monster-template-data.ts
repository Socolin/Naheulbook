import {MonsterAttackData} from './monster-attack-data';
import {MonsterSpellBookData} from './monster-spell-book-data';

export interface MonsterTemplateData {
    attacks: MonsterAttackData[];
    prd?: number;
    esq?: number;
    ev: number;
    ea: number;
    cou: number;
    note: string;
    pr: number;
    pr_magic: number;
    resm: number;
    xp: number;
    chercheNoise: boolean;
    special: boolean;
    traits: {
        traitId: number;
        level: number;
    }[];
    page: number;
    spellBooks?: MonsterSpellBookData[];
}
