import {CharacterSex} from '../shared/enums';

export interface CreateCustomCharacterRequest {
    name: string;
    sex: CharacterSex;
    fatePoint: number;
    level: number;
    experience: number;
    stats: {
        ad: number,
        cou: number,
        cha: number,
        fo: number,
        int: number
    };
    basicStatsOverrides: {
        at: number,
        prd: number,
        ev: number,
        ea: number
    },
    originId: number;
    jobIds: number[];
    skillIds: number[];
    specialityIds: { [jobId: number]: number[] };

    isNpc?: boolean;
    groupId?: number;
}
