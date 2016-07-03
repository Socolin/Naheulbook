import {StatRequirement} from "../shared/stat-requirement.model";

export interface OriginInfo {
    title: string;
    description: string;
}
export interface OriginRestrict {
    text: string;
}
export interface OriginSkill {
    id: number;
    name: string;
}
export interface OriginBonus {
    token: string;
    description: string;
}
export interface Origin {
    id: number;
    name: string;
    description: string;
    advantage: string;
    baseEv: number;
    baseAT: number;
    basePRD: number;
    baseEV : number;
    size: string;
    bonusAT: number;
    bonusPRD: number;
    speedModifier: number;
    requirements: StatRequirement[];
    infos: OriginInfo[];
    restricts: OriginRestrict[];
    restrictsTokens: string[];
    skills: OriginSkill[];
    availableSkills: OriginSkill[];
    bonuses: OriginBonus[];
    specials: string[];
    diceEVLevelUp: number;
}
