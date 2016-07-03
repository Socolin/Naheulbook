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
}
export class ItemSlot {
    id: number;
    name: string;
    techName: string;
}
export class ItemTemplate {
    id: number;
    name: string;
    description: string;
    note: string;
    diceDrop: number;
    price: number;
    category: any;
    modifiers: ItemStatModifier[] = [];
    skills: IMetadata[];
    unskills: IMetadata[];
    container: boolean;
    slots: ItemSlot[];
    slotCount: number;
    throwable: boolean;
    rupture: number;
    level: number;
    damageDiceCount: number;
    damageType: any;
    damage: any;
    magicProtection: number;
    protection: number;
    protectionAgainstMagic: any;
    protectionAgainstType: any;
    charge: any;
    skillModifiers: any[];
    availableLocation: string;
    relic: boolean;
    sex: string;
    quantifiable: boolean;
}