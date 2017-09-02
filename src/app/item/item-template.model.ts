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
    section: ItemSection;
    sectionId: number;
}

export class ItemType {
    id: number;
    displayName: string;
    techName: string;
}

export class ItemSlot {
    id: number;
    name: string;
    techName: string;
}

export class ItemTemplateGunData {
    range: string;
    damages: string;
    special: string;
    rateOfFire: string;
    reloadDelay: string;
    shootTest: string;
    fuelPerShot: string;
    ammunitionPerShot: string;
    workLuck: string;

    static fromJson(jsonData: ItemTemplateGunData): ItemTemplateGunData | undefined {
        if (!jsonData) {
            return undefined;
        }
        let gunData = new ItemTemplateGunData();
        Object.assign(gunData, jsonData);
        return gunData;
    }
}

export class ItemTemplateData {
    actions?: any[];
    availableLocation?: string;
    bonusDamage?: number;
    charge?: number;
    container?: boolean;
    damageDice?: number;
    damageType?: any;
    description?: string;
    diceDrop?: number;
    duration?: string;
    enchantment?: string;
    gun?: ItemTemplateGunData;
    icon?: IconDescription;
    isCurrency?: boolean;
    itemTypes?: string[];
    lifetime?: IDurable;
    magicProtection?: number;
    note?: string;
    notIdentifiedName?: string;
    origin?: string;
    price?: number;
    protection?: number;
    protectionAgainstMagic?: any;
    protectionAgainstType?: any;
    quantifiable?: boolean;
    rarityIndicator?: string;
    relic?: boolean;
    requireLevel?: number;
    rupture?: number;
    sex?: string;
    skillBook?: boolean;
    space?: string;
    throwable?: boolean;
    useUG?: boolean;
    weight?: number;

    static fromJson(jsonData: ItemTemplateData): ItemTemplateData {
        let itemTemplateData = new ItemTemplateData();
        Object.assign(itemTemplateData, jsonData, {gun: ItemTemplateGunData.fromJson(jsonData.gun)});
        return itemTemplateData;
    }

    isItemTypeName(itemTypeName: string): boolean {
        if (!this.itemTypes) {
            return false;
        }
        let i = this.itemTypes.findIndex(name => name === itemTypeName);
        return i !== -1;
    }

    isItemType(itemType: ItemType): boolean {
        if (!this.itemTypes) {
            return false;
        }
        let i = this.itemTypes.findIndex(name => name === itemType.techName);
        return i !== -1;
    }

    toggleItemType(itemType: ItemType): void {
        if (!this.itemTypes) {
            this.itemTypes = [];
        }
        if (this.isItemType(itemType)) {
            let i = this.itemTypes.findIndex(name => name === itemType.techName);
            if (i !== -1) {
                this.itemTypes.splice(i, 1);
            }
        } else {
            this.itemTypes.push(itemType.techName);
        }
    }
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
    sourceUser?: string;
    sourceUserId?: number;
    modifiers: ItemStatModifier[] = [];
    skills: Skill[] = [];
    unskills: Skill[] = [];
    slots: ItemSlot[] = [];
    restrictJobs: Job[] = [];
    requirements: any[] = [];
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
        Object.assign(itemTemplate, jsonData, {
            skills: [],
            unskills: [],
            skillModifiers: [],
            data: ItemTemplateData.fromJson(jsonData.data)
        });

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
        let itemTemplates: ItemTemplate[] = [];
        for (let jsonData of jsonDatas) {
            itemTemplates.push(ItemTemplate.fromJson(jsonData, skillsById));
        }
        return itemTemplates;
    }

    isInSlot(slot: ItemSlot): boolean {
        if (!this.slots) {
            return false;
        }
        let i = this.slots.findIndex(s => s.id === slot.id);
        return i !== -1;
    }

    toggleSlot(slot: ItemSlot): void {
        if (!this.slots) {
            this.slots = [];
        }
        if (this.isInSlot(slot)) {
            let i = this.slots.findIndex(s => s.id === slot.id);
            if (i !== -1) {
                this.slots.splice(i, 1);
            }
        } else {
            this.slots.push(slot);
        }
    }
}
