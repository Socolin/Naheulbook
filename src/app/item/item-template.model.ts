import {ItemStatModifier} from '../shared/stat-modifier.model';
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
    notIdentifiedName: string;
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
    useUG: boolean;
    lifetimeType: string;
    lifetime: string|number;
}
// FIXME: Is this used and checked ?
export class ItemRequirement {
    stat: string;
    min: number;
    max: number;
}
export class ItemSkillModifier {
    skill: number|Skill;
    value: number;
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
    skillModifiers: ItemSkillModifier[];

    static hasSlot(template: ItemTemplate, slotName: string): boolean {
        if (!template.slots) {
            return false;
        }
        for (let i = 0; i < template.slots.length; i++) {
            let slot = template.slots[i];
            if (slot == null) {
                continue;
            }
            if (template.slots[i].techName === slotName) {
                return true;
            }
        }
        return false;
    }
}
