import {IMonsterData} from '../shared';

export class DeadMonsterResponse {
    id: number;
    name: string;
    dead?: string;
    data: IMonsterData;
}
