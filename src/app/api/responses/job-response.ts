import {FlagResponse} from './flag-response';
import {DescribedFlagResponse} from './described-flag-response';
import {SpecialityResponse} from './speciality-response';

export interface JobResponse {
    id: number;
    name: string;
    internalname?: string;
    informations?: string;
    playerDescription?: string;
    playerSummary?: string;
    maxLoad?: number;
    maxArmorPR?: number;
    isMagic?: boolean;
    baseEv?: number;
    factorEv?: number;
    bonusEv?: number;
    baseEa?: number;
    diceEaLevelUp?: number;
    baseAT?: number;
    basePRD?: number;
    parentJobId?: number;
    flags?: FlagResponse[];
    skillIds: number[];
    availableSkillIds: number[];
    originsWhitelist: { id: number, name: string }[];
    originsBlacklist: { id: number, name: string }[];
    bonuses: DescribedFlagResponse[];
    requirements: StatRequirementResponse[];
    restricts: DescribedFlagResponse[];
    specialities: SpecialityResponse[];
}
