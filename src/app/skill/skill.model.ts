import {Flag, FlagData, StatModifier} from '../shared';
import {SkillResponse} from '../api/responses';

export type SkillDictionary =  { [skillId: number]: Skill };
export class Skill {
    id: number;
    name: string;
    description: string;
    playerDescription: string;
    require: string;
    resist: string;
    using: string;
    roleplay: string;
    stat: string[];
    test?: string;
    effects: StatModifier[];
    flags: Flag[];

    static fromResponse(response: SkillResponse): Skill {
        let skill = new Skill();
        Object.assign(skill, response);
        return skill;
    }

    hasFlag(flagName: string): boolean {
        if (!this.flags) {
            return false;
        }
        let i = this.flags.findIndex(f => f.type === flagName);
        return i !== -1;
    }

    getFlagsDatas(data: { [flagName: string]: FlagData[] }): void {
        if (!this.flags) {
            return;
        }
        for (let flag of this.flags) {
            if (!(flag.type in data)) {
                data[flag.type] = [];
            }
            data[flag.type].push({data: flag.data, source: {type: 'skill', name: this.name}});
        }
    }
}
