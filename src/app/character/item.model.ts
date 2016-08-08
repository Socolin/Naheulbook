import {ItemTemplate} from "../item";
import {IMetadata} from '../shared/misc.model';

export class ItemData {
    name: string;
    description: string;
    quantity: number;
    charge: number;
    equiped: number;
    readCount: number;
}
export class Item {
    id: number;
    data: ItemData;
    container: number;
    template: ItemTemplate;
    // Generated field
    content: Item[];
    containerInfo: IMetadata;
}
