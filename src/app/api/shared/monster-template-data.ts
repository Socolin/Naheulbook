export interface MonsterTemplateData {
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
    traits: {
        traitId: number;
        level: number;
    }[];
    page: number;
}
