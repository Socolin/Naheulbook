import {FlagResponse} from './flag-response';
import {IStatModifier} from '../shared';

export interface SpecialityResponse {
    id: number;
    name: string;
    description: string;
    modifiers: IStatModifier[];
    specials: {
        id: number;
        isBonus: boolean;
        description: string;
        flags?: FlagResponse[];
    }[];
    flags?: FlagResponse[];
}

