import {ItemTemplateCategoryResponse} from './item-template-category-response';

export interface ItemTemplateSectionResponse {
    id: number;
    name: string;
    note: string;
    specials: string[];
    categories: ItemTemplateCategoryResponse[];
}
