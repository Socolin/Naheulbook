import {StatRequirement} from '../shared/stat-requirement.model';
import {Skill} from '../skill';
import {Flag, DescribedFlag} from '../shared';

import {Speciality} from './speciality.model';

export class Job {
    availableSkills: Array<Skill>;
    baseAT: number|null;
    baseEa: number|null;
    baseEv: number|null;
    basePRD: number|null;
    bonuses: Array<DescribedFlag>;
    bonusEv: number|null;
    description: string;
    diceEaLevelUp: number|null;
    factorEv: number|null;
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
        });

        for (let s of jobData.skills) {
            job.skills.push(skillsById[s.id]);
        }
        for (let s of jobData.availableSkills) {
            job.availableSkills.push(skillsById[s.id]);
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

    getFlagDatas(flagName: string): any[] {
        let data: any[] = [];

        for (let restrict of this.restricts) {
            let d = restrict.getFlagDatas(flagName);
            data.push(...d);
        }

        for (let bonus of this.bonuses) {
            let d = bonus.getFlagDatas(flagName);
            data.push(...d);
        }

        for (let flag of this.flags) {
            if (flag.type === flagName) {
                if (flag.data) {
                    data.push(flag.data);
                }
            }
        }

        return data;
    }
}
