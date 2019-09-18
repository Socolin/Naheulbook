import {MonsterTemplateData} from '../shared';

export interface MonsterInventoryElementRequest {
    itemTemplate: { id: number };
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
    name: string;
    data: MonsterTemplateData;
    simpleInventory: MonsterInventoryElementRequest[];
    locations: number[];
}

export interface EditMonsterTemplateRequest extends MonsterTemplateRequest {

}

export interface CreateMonsterTemplateRequest {
    categoryId: number;
    monster: MonsterTemplateRequest;
}
