import {ItemTemplate} from "../item";
import {IMetadata} from '../shared/misc.model';

export class ItemData {
    name: string;
    description: string;
    quantity: number;
    charge: number;
    ug: number;
    equiped: number;
    readCount: number;
    notIdentified: boolean;
}
export class Item {
    id: number;
    data: ItemData = new ItemData();
    container: number;
    template: ItemTemplate;
    // Generated field
    content: Item[];
    containerInfo: IMetadata;
}

export class PartialItem {
    id: number;
    data: ItemData = new ItemData();
    container: number;
}
