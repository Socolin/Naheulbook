import {EffectCategoryResponse} from './effect-category-response';

export interface EffectTypeResponse {
    id: number;
    name: string;
    categories: EffectCategoryResponse[];
}
