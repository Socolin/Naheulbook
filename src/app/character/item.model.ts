import {ItemTemplate} from '../item';
import {IMetadata} from '../shared/misc.model';
import {IconDescription} from '../shared/icon.model';
import {StatModifier} from '../shared/stat-modifier.model';

export class ItemData {
    name: string;
    description: string;
    quantity: number;
    icon: IconDescription;
    charge: number;
    ug: number;
    equiped: number;
    readCount: number;
    notIdentified: boolean;
    lifetimeType: string;
    lifetime: string|number;
}

export class ItemModifier {
    name: string;
    durationType: string = 'combat';
    duration: string|number = 1;
    active: boolean;
    currentDuration: any;
    reusable: boolean = false;
    values: StatModifier[] = [];
}

export class Item {
    id: number;
    data: ItemData = new ItemData();
    modifiers: ItemModifier[];
    container: number;
    template: ItemTemplate;
    // Generated field
    content: Item[];
    containerInfo: IMetadata;
}

export class PartialItem {
    id: number;
    data: ItemData = new ItemData();
    modifiers: ItemModifier[];
    container: number;
}
