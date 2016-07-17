import {ItemTemplate} from "../item";

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
    container: Item;
    template: ItemTemplate;
    // Generated field
    content: Item[];
}
