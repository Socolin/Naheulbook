import {ItemTemplateResponse} from './item-template-response';
import {MonsterTemplateData} from '../shared';

export interface MonsterTemplateResponse {
    id: number;
    name: string;
    subCategoryId: number;
    data: MonsterTemplateData;
    simpleInventory: MonsterSimpleInventoryResponse[];
}

export interface MonsterSimpleInventoryResponse {
    id: number;
    itemTemplate: ItemTemplateResponse;
    minCount: number;
    maxCount: number;
    chance: number;
    hidden: boolean;
}
