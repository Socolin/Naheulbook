import {IActiveStatsModifier, IMonsterData} from '../shared';
import {CreateItemRequest} from './create-item-request';

export interface CreateMonsterRequest {
    name: string;
    fightId?: number;
    data: IMonsterData;
    modifiers?: IActiveStatsModifier[];
    items: CreateItemRequest[]
}
