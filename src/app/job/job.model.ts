import {StatRequirement} from "../shared/stat-requirement.model";
import {Speciality} from "../character";

interface JobSkill {
    id: number;
    name: string;
}
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
    skills: Array<JobSkill>;
    restricts: Array<JobRestrict>;
    availableSkills: Array<JobSkill>;
    bonuses: Array<JobBonus>;
    specialities: Array<Speciality>;
    specials: string[];
    diceEALevelUp: number;
}
