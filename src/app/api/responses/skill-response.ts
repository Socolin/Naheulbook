import {FlagResponse} from './flag-response';
import {StatModificationOperand} from '../shared/enums';

export interface SkillResponse {
    id: number;
    name: string;
    description?: string;
    playerDescription?: string;
    require?: string;
    resist?: string;
    using?: string;
    roleplay?: string;
    stat?: string[];
    test?: number;
    flags?: FlagResponse[];
    effects: {
        stat: string;
        value: number;
        type: StatModificationOperand;
    }[];
}
