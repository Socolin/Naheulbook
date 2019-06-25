import {StatRequirement} from '../shared/stat-requirement.model';
import {Skill} from '../skill';
import {Flag, FlagData, DescribedFlag} from '../shared';

import {Speciality} from './speciality.model';

export class Job {
    availableSkills: Array<Skill>;
    baseAT?: number;
    baseEa?: number;
    baseEv?: number;
    basePRD?: number;
    bonuses: Array<DescribedFlag>;
    bonusEv?: number;
    description: string;
    diceEaLevelUp?: number;
    factorEv?: number;
    id: number;
    information: string;
    playerDescription: string;
    playerSummary: string;
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
    maxLoad?: number; // FIXME Not used
    maxArmorPR?: number; // FIXME Not used
    parentJob: Job;
    parentJobId: number;
    requirements: Array<StatRequirement>;
    restricts: Array<DescribedFlag>;
    skills: Array<Skill>;
    specialities: Array<Speciality>;
    flags: Flag[] = [];

    static fromJson(jobData: any, skillsById: {[skillId: number]: Skill}): Job {
        let job = new Job();

        Object.assign(job, jobData, {
            skills: [],
            availableSkills: [],
            bonuses: DescribedFlag.flagsFromJson(jobData.bonuses),
            restricts: DescribedFlag.flagsFromJson(jobData.restricts),
            specialities: Speciality.specialitiesFromJson(jobData.specialities),
        });

        for (let skillId of jobData.skillIds) {
            job.skills.push(skillsById[skillId]);
        }
        for (let skillId of jobData.availableSkillIds) {
            job.availableSkills.push(skillsById[skillId]);
        }

        if (!job.flags) {
            job.flags = [];
        }

        return job;
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

    getFlagsDatas(data: {[flagName: string]: FlagData[]}): void {
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
