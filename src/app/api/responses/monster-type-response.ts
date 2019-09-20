import {MonsterCategoryResponse} from './monster-category-response';

export interface MonsterTypeResponse {
    id: number;
    name: string;
    categories: MonsterCategoryResponse[];
}
