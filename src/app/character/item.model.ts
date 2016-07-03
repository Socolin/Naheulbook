import {ItemTemplate} from "../item";

export class Item {
    id: number;
    name: string;
    description: string;
    quantity: number;
    charge: number;
    equiped: number;
    container: Item;
    template: ItemTemplate;
    // Generated field
    content: Item[];
}