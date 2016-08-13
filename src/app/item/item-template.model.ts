import {ItemStatModifier} from "../shared/stat-modifier.model";
import {Skill} from '../skill';

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
    damageDice: number;
    damageType: any;
    bonusDamage: number;
    magicProtection: number;
    protection: number;
    protectionAgainstMagic: any;
    protectionAgainstType: any;
    charge: any;
    availableLocation: string;
    requireLevel: number;
    relic: boolean;
    sex: string;
    quantifiable: boolean;
    skillBook: boolean;
    weight: number;
    duration: string;
    space: string;
}
// FIXME: Is this used and checked ?
export class ItemRequirement {
    stat: string;
    min: number;
    max: number;
}
export class ItemTemplate {
    id: number;
    name: string;
    category: number;
    data: ItemTemplateData = new ItemTemplateData();
    modifiers: ItemStatModifier[] = [];
    skills: Skill[];
    unskills: Skill[];
    slots: ItemSlot[];
    requirements: any[];
    skillModifiers: any[];
}
