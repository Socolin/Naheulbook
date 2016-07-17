import {StatRequirement} from "../shared/stat-requirement.model";
import {Speciality} from "../character";
import {IMetadata} from '../shared/misc.model';

interface JobRestrict {
    text: string;
}
interface JobBonus {
    token: string;
    description: string;
}

export interface Job {
    id: number;
    name: string;
    description: string;
    baseEV: number;
    factorEV: number;
    bonusEV: number;
    baseEA: number;
    diceEaLevelUp: number;
    baseAT: number;
    basePRD: number;
    isMagic: boolean;
    originsBlacklist: Array<{
        id: number,
        name: string
    }>;
    originsWhitelist: Array<{
        id: number,
        name: string
    }>;
    parentJob: Job;
    requirements: Array<StatRequirement>;
    skills: Array<IMetadata>;
    restricts: Array<JobRestrict>;
    availableSkills: Array<IMetadata>;
    bonuses: Array<JobBonus>;
    specialities: Array<Speciality>;
    specials: string[];
    diceEALevelUp: number;
}
