import {ItemTemplate} from '../item';
import {IMetadata} from '../shared/misc.model';
import {IconDescription} from '../shared/icon.model';
import {StatsModifier} from '../shared/stat-modifier.model';
import {DurationType, IDurable} from '../date/durable.model';

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
    lifetime: IDurable;
}

export class ItemModifier extends StatsModifier {
    active: boolean;

    currentCombatCount: number;
    currentTimeDuration: number;
    currentLapCount: number;
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

    static fromJson(jsonData: any): Item {
        let item = new Item();
        Object.assign(item, jsonData);
        return item;
    }

    static itemsFromJson(itemsData: object[]): Item[] {
        let items: Item[] = [];
        for (let i = 0; i < itemsData.length; i++) {
            items.push(Item.fromJson(itemsData[i]));
        }
        return items;
    }
}

export class PartialItem {
    id: number;
    data: ItemData = new ItemData();
    modifiers: ItemModifier[];
    container: number;

    static fromJson(jsonData: any): PartialItem {
        let item = new PartialItem();
        Object.assign(item, jsonData);
        return item;
    }
}
