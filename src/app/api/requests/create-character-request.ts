import {Guid} from '../shared/util';

export interface CreateCharacterRequest {
    name: string;
    sex: string;
    money: number;
    fatePoint: number;
    isNpc?: boolean;
    groupId?: number;
    stats: { [key: string]: number };
    modifiedStat: {
        [key: string]: {
            name: string;
            stats: { [key: string]: number };
        }
    };
    job?: number;
    origin: number;
    skillIds: Guid[];
    speciality?: number;
}
