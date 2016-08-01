import {StatRequirement} from "../shared/stat-requirement.model";
import {Skill} from '../skill/skill.model';

export interface OriginInfo {
    title: string;
    description: string;
}
export interface OriginRestrict {
    text: string;
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
    basePRD: number;
    baseEV: number;
    size: string;
    bonusAT: number;
    bonusPRD: number;
    speedModifier: number;
    requirements: StatRequirement[];
    infos: OriginInfo[];
    restricts: OriginRestrict[];
    restrictsTokens: string[];
    skills: Skill[];
    availableSkills: Skill[];
    bonuses: OriginBonus[];
    specials: string[];
    diceEVLevelUp: number;
}
