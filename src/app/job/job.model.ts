import {StatRequirement} from "../shared/stat-requirement.model";
import {Speciality} from "../character";
import {Skill} from '../skill';

interface JobRestrict {
    text: string;
}

interface JobBonus {
    token: string;
    description: string;
}

export interface Job {
    availableSkills: Array<Skill>;
    baseAT: number;
    baseEa: number;
    baseEv: number;
    basePRD: number;
    bonuses: Array<JobBonus>;
    bonusEv: number;
    description: string;
    diceEaLevelUp: number;
    factorEv: number;
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
