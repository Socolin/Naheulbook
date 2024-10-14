import {MonsterResponse} from './monster-response';

export type FightResponse = {
    id: number;
    name: string;
    monsters: MonsterResponse[];
};
