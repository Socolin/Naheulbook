import {StatModifier} from '../shared';

export interface Skill {
    id: number;
    name: string;
    description: string;
    playerDescription: string;
    require: string;
    resist: string;
    using: string;
    roleplay: string;
    stat: string;
    test: string;
    effects: StatModifier[];
}
