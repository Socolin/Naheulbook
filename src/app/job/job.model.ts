import {DescribedFlag, Flag, FlagData, StatRequirement} from '../shared';
import {Skill, SkillDictionary} from '../skill';

import {Speciality} from './speciality.model';
import {JobResponse} from '../api/responses';

export type JobDictionary = { [jobId: number]: Job };

export class Job {
    readonly availableSkills: Array<Skill>;
    readonly baseAT?: number;
    readonly baseEa?: number;
    readonly baseEv?: number;
    readonly basePRD?: number;
    readonly bonuses: Array<DescribedFlag>;
    readonly bonusEv?: number;
    readonly diceEaLevelUp?: number;
    readonly factorEv?: number;
    readonly id: number;
    readonly playerDescription?: string;
    readonly playerSummary?: string;
    readonly isMagic: boolean;
    readonly name: string;
    originsBlacklist: Array<{
        id: number,
        name: string
    }>;
    originsWhitelist: Array<{
        id: number,
        name: string
    }>;
    readonly maxLoad?: number; // FIXME Not used
    readonly maxArmorPR?: number; // FIXME Not used
    readonly parentJobId?: number;
    readonly requirements: Array<StatRequirement>;
    readonly restricts: Array<DescribedFlag>;
    readonly skills: Array<Skill>;
    readonly specialities: Array<Speciality>;
    readonly flags: Flag[] = [];

    static fromResponse(response: JobResponse, skillsById: SkillDictionary): Job {
        const job = new Job(response, skillsById);
        // Object.freeze(job);
        return job;
    }

    constructor(response: JobResponse, skillsById: SkillDictionary) {
        this.baseAT = response.baseAT;
        this.baseEa = response.baseEa;
        this.baseEv = response.baseEv;
        this.basePRD = response.basePRD;
        this.bonuses = DescribedFlag.flagsFromJson(response.bonuses);
        this.bonusEv = response.bonusEv;
        this.diceEaLevelUp = response.diceEaLevelUp;
        this.factorEv = response.factorEv;
        this.id = response.id;
        this.playerDescription = response.playerDescription;
        this.playerSummary = response.playerSummary;
        this.isMagic = response.isMagic || false;
        this.name = response.name;
        this.originsBlacklist = response.originsBlacklist;
        this.originsWhitelist = response.originsWhitelist;
        this.maxLoad = response.maxLoad;
        this.maxArmorPR = response.maxArmorPR;
        this.parentJobId = response.parentJobId;
        this.requirements = response.requirements;
        this.restricts = DescribedFlag.flagsFromJson(response.restricts);
        this.skills = response.skillIds.map(skillId => skillsById[skillId]);
        this.availableSkills = response.availableSkillIds.map(skillId => skillsById[skillId]);
        this.specialities = Speciality.fromResponses(response.specialities);
        this.flags = response.flags || [];
    }

    hasFlag(flagName: string): boolean {
        for (let restrict of this.restricts) {
            if (restrict.hasFlag(flagName)) {
                return true;
            }
        }

        for (let bonus of this.bonuses) {
            if (bonus.hasFlag(flagName)) {
                return true;
            }
        }

        let i = this.flags.findIndex(f => f.type === flagName);
        if (i !== -1) {
            return true;
        }

        return false;
    }

    getFlagsDatas(data: { [flagName: string]: FlagData[] }): void {
        for (let restrict of this.restricts) {
            restrict.getFlagDatas(data, {type: 'job', name: this.name});
        }

        for (let bonus of this.bonuses) {
            bonus.getFlagDatas(data, {type: 'job', name: this.name});
        }

        for (let flag of this.flags) {
            if (!(flag.type in data)) {
                data[flag.type] = [];
            }
            data[flag.type].push({data: flag.data, source: {type: 'job', name: this.name}});
        }
    }
}
