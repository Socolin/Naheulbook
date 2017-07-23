import {ItemStatModifier} from '../shared/stat-modifier.model';
import {Skill} from '../skill';
import {Job} from '../job';
import {IconDescription} from '../shared/icon.model';
import {IDurable} from '../date/durable.model';

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
    actions: any[];
    description: string;
    note: string;
    notIdentifiedName: string;
    diceDrop: number;
    price: number;
    container: boolean;
    isCurrency: boolean;
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
    lifetime: IDurable;
    enchantment?: string;
    icon: IconDescription;
}

// FIXME: Is this used and checked ?
export class ItemRequirement {
    stat: string;
    min: number;
    max: number;
}

export class ItemSkillModifierJsonData {
    skill: number;
    value: number;
}

export class ItemTemplateJsonData {
    id: number;
    name: string;
    category: number;
    data: ItemTemplateData;
    modifiers: ItemStatModifier[] = [];
    skills: {id: number}[];
    unskills: {id: number}[];
    slots: ItemSlot[];
    restrictJobs: {id: number}[];
    requirements: ItemRequirement[];
    skillModifiers: ItemSkillModifierJsonData[];
}

export class ItemSkillModifier {
    skill: Skill;
    value: number;

    constructor(skill: Skill, value: number) {
        this.skill = skill;
        this.value = value;
    }
}

export class ItemTemplate {
    id: number;
    name: string;
    category: number;
    data: ItemTemplateData = new ItemTemplateData();
    source: 'official'|'community'|'private';
    sourceUser: string;
    sourceUserId: number;
    modifiers: ItemStatModifier[] = [];
    skills: Skill[];
    unskills: Skill[];
    slots: ItemSlot[];
    restrictJobs: Job[];
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

    static fromJson(jsonData: ItemTemplateJsonData, skillsById: {[skillId: number]: Skill}): ItemTemplate {
        let itemTemplate = new ItemTemplate();
        Object.assign(itemTemplate, jsonData, {skills: [], unskills: [], skillModifiers: []});

        for (let s of jsonData.skills) {
            itemTemplate.skills.push(skillsById[s.id]);
        }
        for (let s of jsonData.unskills) {
            itemTemplate.unskills.push(skillsById[s.id]);
        }

        for (let skillModifier of jsonData.skillModifiers) {
            itemTemplate.skillModifiers.push(new ItemSkillModifier(skillsById[+skillModifier.skill], skillModifier.value));
        }

        return itemTemplate;
    }

    static itemTemplatesFromJson(jsonDatas: ItemTemplateJsonData[], skillsById: {[skillId: number]: Skill}): ItemTemplate[] {
        let itemTemplates = [];
        for (let jsonData of jsonDatas) {
            itemTemplates.push(ItemTemplate.fromJson(jsonData, skillsById));
        }
        return itemTemplates;
    }
}
