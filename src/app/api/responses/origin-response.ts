import {DescribedFlagResponse} from './described-flag-response';
import {FlagResponse} from './flag-response';

export interface OriginResponse {
    id: number;
    name: string;
    description: string;
    playerDescription: string;
    playerSummary: string;
    maxLoad?: number;
    maxArmorPR?: number;
    advantage: string;
    baseEV?: number;
    baseEA?: number;
    bonusAT?: number;
    bonusPRD?: number;
    diceEVLevelUp: number;
    size: string;
    flags: FlagResponse[];
    speedModifier?: number;
    skillIds: number[];
    availableSkillIds: number[];
    infos: {
        title: string;
        description: string;
    }[];
    bonuses: DescribedFlagResponse[];
    requirements: {
        stat: string;
        min: number | null;
        max: number | null;
    }[];
    restricts: DescribedFlagResponse[];
}
