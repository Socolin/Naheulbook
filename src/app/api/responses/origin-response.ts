import {DescribedFlagResponse} from './described-flag-response';
import {FlagResponse} from './flag-response';
import {OriginData} from '../shared/origin-data';

export interface OriginResponse {
    id: number;
    name: string;
    data: OriginData;
    description: string;
    playerDescription?: string;
    playerSummary?: string;
    advantage?: string;
    size?: string;
    flags: FlagResponse[];
    skillIds: number[];
    availableSkillIds: number[];
    information: {
        title: string;
        description: string;
    }[];
    bonuses: DescribedFlagResponse[];
    requirements: {
        stat: string;
        min?: number;
        max?: number;
    }[];
    restrictions: DescribedFlagResponse[];
}
