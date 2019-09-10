import {IItemData} from '../shared';

export interface CreateItemRequest {
    itemTemplateId: number;
    itemData: IItemData;
}
