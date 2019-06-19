import {DescribedFlag, Flag, FlagData, StatRequirement} from '../shared';
import {Skill} from '../skill';

export interface OriginInfo {
    title: string;
    description: string;
}

export class Origin {
    id: number;
    name: string;
    description: string;
    playerDescription: string;
    playerSummary: string;
    advantage: string;
    basePRD: number;
    baseEV: number;
    size: string;
    bonusAT: number;
    bonusPRD: number;
    speedModifier: number;
    requirements: StatRequirement[];
    infos: OriginInfo[];
    restrictsTokens: string[];
    skills: Skill[];
    availableSkills: Skill[];
    bonuses: DescribedFlag[];
    restricts: DescribedFlag[];
    flags: Flag[] = [];
    diceEVLevelUp: number;

    static fromJson(originData: any, skillsById: {[skillId: number]: Skill}): Origin {
        let origin = new Origin();

        Object.assign(origin, originData, {
            skills: [],
            availableSkills: [],
            bonuses: DescribedFlag.flagsFromJson(originData.bonuses),
            restricts: DescribedFlag.flagsFromJson(originData.restricts),
        });

        for (let skillId of originData.skillIds) {
            origin.skills.push(skillsById[skillId]);
        }
        for (let skillId of originData.availableSkillIds) {
            origin.availableSkills.push(skillsById[skillId]);
        }

        if (!origin.flags) {
            origin.flags = [];
        }

        return origin;
    }

    hasFlag(flagName: string): boolean {
        let i = this.flags.findIndex(f => f.type === flagName);
        if (i !== -1) {
            return true;
        }

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

        return false;
    }

    getFlagsDatas(data: {[flagName: string]: FlagData[]}): void {
        for (let restrict of this.restricts) {
            restrict.getFlagDatas(data, {type: 'origin', name: this.name});
        }

        for (let bonus of this.bonuses) {
            bonus.getFlagDatas(data, {type: 'origin', name: this.name});
        }

        for (let flag of this.flags) {
            if (!(flag.type in data)) {
                data[flag.type] = [];
            }
            data[flag.type].push({data: flag.data, source: {type: 'origin', name: this.name}});
        }
    }
}
