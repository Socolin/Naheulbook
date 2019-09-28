import {IActiveStatsModifier} from '../shared';
import {ItemResponse} from './item-response';
import {SpecialityResponse} from './speciality-response';
import {CharacterGroupInviteResponse} from './character-group-invite-response';

export interface CharacterResponse {
    id: number;
    name: string;
    sex: string;
    originId: number;
    isNpc: boolean;
    ev?: number;
    ea?: number;
    level: number;
    experience: number;
    fatePoint: number;
    stats: {
        AD: number;
        COU: number;
        CHA: number;
        FO: number;
        INT: number;
    };
    statBonusAd?: string;
    jobIds: number[];
    skillIds: number[];
    group?: {
        name: string;
        id: number;
    };
    modifiers: IActiveStatsModifier[];
    specialities: SpecialityResponse[];
    items: ItemResponse[];
    invites: CharacterGroupInviteResponse[];
}

export interface CharacterFoGmResponse extends CharacterResponse {
    isActive: boolean;
    color: string;
    gmData?: {
        mankdebol: number;
        debilibeuk: number;
    };
    ownerId?: number;
    target?: {
        id: number;
        isMonster: boolean;
    };
}
