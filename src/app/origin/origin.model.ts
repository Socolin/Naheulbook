import {DescribedFlag, Flag, FlagData, StatRequirement} from '../shared';
import {Skill, SkillDictionary} from '../skill';
import {OriginResponse} from '../api/responses';

export interface OriginInfo {
    title: string;
    description: string;
}

export type OriginDictionary = { [originId: number]: Origin };

export class Origin {
    readonly id: number;
    readonly name: string;
    readonly description: string;
    readonly playerDescription?: string;
    readonly playerSummary?: string;
    readonly advantage?: string;
    readonly baseEV: number;
    readonly size?: string;
    readonly bonusAT: number;
    readonly bonusPRD: number;
    readonly speedModifier?: number;
    readonly requirements: StatRequirement[];
    readonly infos: OriginInfo[];
    readonly skills: Skill[];
    readonly availableSkills: Skill[];
    readonly bonuses: DescribedFlag[];
    readonly restricts: DescribedFlag[];
    readonly flags: Flag[] = [];
    readonly diceEVLevelUp: number;

    static fromResponse(response: OriginResponse, skillsById: SkillDictionary): Origin {
        const origin = new Origin(response, skillsById);
        Object.freeze(origin);
        return origin;
    }

    private constructor(response: OriginResponse, skillsById: SkillDictionary) {
        this.id = response.id;
        this.name = response.name;
        this.description = response.description;
        this.playerDescription = response.playerDescription;
        this.playerSummary = response.playerSummary;
        this.advantage = response.advantage;
        this.baseEV = response.baseEV;
        this.size = response.size;
        this.bonusAT = response.bonusAT || 0;
        this.bonusPRD = response.bonusPRD || 0;
        this.speedModifier = response.speedModifier;
        this.requirements = response.requirements;
        this.infos = response.infos;
        this.skills = response.skillIds.map(skillId => skillsById[skillId]);
        this.availableSkills = response.availableSkillIds.map(skillId => skillsById[skillId]);
        this.bonuses = DescribedFlag.flagsFromJson(response.bonuses);
        this.restricts = DescribedFlag.flagsFromJson(response.restricts);
        this.flags = response.flags || [];
        this.diceEVLevelUp = response.diceEVLevelUp;
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
