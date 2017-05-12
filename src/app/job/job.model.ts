import {StatRequirement} from '../shared/stat-requirement.model';
import {Speciality} from '../character';
import {Skill} from '../skill';

export interface JobRestrict {
    text: string;
}

export  interface JobBonus {
    token: string;
    description: string;
}

export interface Job {
    availableSkills: Array<Skill>;
    baseAT: number|null;
    baseEa: number|null;
    baseEv: number|null;
    basePRD: number|null;
    bonuses: Array<JobBonus>;
    bonusEv: number|null;
    description: string;
    diceEaLevelUp: number|null;
    factorEv: number|null;
    id: number;
    information: string;
    isMagic: boolean;
    name: string;
    originsBlacklist: Array<{
        id: number,
        name: string
    }>;
    originsWhitelist: Array<{
        id: number,
        name: string
    }>;
    parentJob: Job;
    parentJobId: number;
    requirements: Array<StatRequirement>;
    restricts: Array<JobRestrict>;
    skills: Array<Skill>;
    specialities: Array<Speciality>;
    specials: string[];
}
