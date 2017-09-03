import {DescribedFlag, Flag, StatRequirement} from '../shared';
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

        for (let s of originData.skills) {
            origin.skills.push(skillsById[s.id]);
        }
        for (let s of originData.availableSkills) {
            origin.availableSkills.push(skillsById[s.id]);
        }

        if (!origin.flags) {
            origin.flags = [];
        }

        return origin;
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
