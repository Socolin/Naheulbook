import {ItemStatModifier} from "../shared/stat-modifier.model";
import {IMetadata} from "../shared/misc.model";

export class ItemSection {
    id: number;
    name: string;
    note: string;
    special: string[];
    categories: ItemCategory[];
}
export class ItemCategory {
    id: number;
    name: string;
    description: string;
    note: string;
    type: any;
}
export class ItemSlot {
    id: number;
    name: string;
    techName: string;
}
export class ItemTemplateData {
    description: string;
    note: string;
    diceDrop: number;
    price: number;
    container: boolean;
    throwable: boolean;
    rupture: number;
    level: number;
    damageDice: number;
    damageType: any;
    bonusDamage: number;
    magicProtection: number;
    protection: number;
    protectionAgainstMagic: any;
    protectionAgainstType: any;
    charge: any;
    availableLocation: string;
    relic: boolean;
    sex: string;
    quantifiable: boolean;
    skillBook: boolean;
}
export class ItemTemplate {
    id: number;
    name: string;
    category: ItemCategory;
    data: ItemTemplateData;
    modifiers: ItemStatModifier[] = [];
    skills: IMetadata[];
    unskills: IMetadata[];
    slots: ItemSlot[];
    skillModifiers: any[];
}
