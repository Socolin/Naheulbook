import {MonsterTemplateData} from '../shared';

export interface MonsterInventoryElementRequest {
    itemTemplateId: number;
    minCount: number;
    maxCount: number;
    chance: number;
    hidden: boolean;
    minUg?: number;
    maxUg?: number;
}

export interface EditMonsterInventoryElementRequest extends MonsterInventoryElementRequest {
    id: number;
}

export interface MonsterTemplateRequest {
    subCategoryId: number;
    name: string;
    data: MonsterTemplateData;
    simpleInventory: MonsterInventoryElementRequest[];
}

