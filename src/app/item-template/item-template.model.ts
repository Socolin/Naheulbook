import {ItemStatModifier} from '../shared';
import {Skill, SkillDictionary} from '../skill';
import {Job} from '../job';
import {IconDescription} from '../shared/icon.model';
import {IDurable, IItemTemplateData, IItemTemplateGunData} from '../api/shared';
import {
    ItemTemplateCategoryResponse,
    ItemTemplateResponse,
    ItemTemplateSectionResponse,
    ItemTypeResponse
} from '../api/responses';

export class ItemTemplateSection {
    id: number;
    name: string;
    note: string;
    specials: string[];
    categories: ItemTemplateCategory[];

    private static fromResponse(response: ItemTemplateSectionResponse): ItemTemplateSection {
        const itemSection = new ItemTemplateSection();
        itemSection.id = response.id;
        itemSection.name = response.name;
        itemSection.note = response.note;
        itemSection.specials = response.specials;
        itemSection.categories = ItemTemplateCategory.fromResponses(response.categories, itemSection);
        return itemSection;
    }

    static fromResponses(responses: ItemTemplateSectionResponse[]): ItemTemplateSection[] {
        return responses.map(response => ItemTemplateSection.fromResponse(response));
    }
}

export type ItemTemplateCategoryDictionary = { [categoryId: number]: ItemTemplateCategory };

export class ItemTemplateCategory {
    id: number;
    name: string;
    description: string;
    note: string;
    section: ItemTemplateSection;
    sectionId: number;

    static fromResponses(responses: ItemTemplateCategoryResponse[], itemTemplateSection: ItemTemplateSection): ItemTemplateCategory[] {
        return responses.map(response => ItemTemplateCategory.fromResponse(response, itemTemplateSection));
    }

    private static fromResponse(response: ItemTemplateCategoryResponse, itemTemplateSection: ItemTemplateSection): ItemTemplateCategory {
        const itemTemplateCategory = new ItemTemplateCategory();
        itemTemplateCategory.id = response.id;
        itemTemplateCategory.name = response.name;
        itemTemplateCategory.description = response.description;
        itemTemplateCategory.note = response.note;
        itemTemplateCategory.sectionId = response.sectionId;
        itemTemplateCategory.section = itemTemplateSection;
        return itemTemplateCategory;
    }
}

export class ItemSlot {
    id: number;
    name: string;
    techName: string;
}

export class ItemTemplateGunData implements IItemTemplateGunData {
    range: string;
    damages: string;
    special: string;
    rateOfFire: string;
    reloadDelay: string;
    shootTest: string;
    fuelPerShot: string;
    ammunitionPerShot: string;
    workLuck: string;

    static fromResponse(response?: IItemTemplateGunData): ItemTemplateGunData | undefined {
        if (!response) {
            return undefined;
        }
        let gunData = new ItemTemplateGunData();
        Object.assign(gunData, response);
        return gunData;
    }
}

export class ItemTemplateData implements IItemTemplateData {
    actions?: any[];
    availableLocation?: string;
    bonusDamage?: number;
    bruteWeapon?: boolean;
    charge?: number;
    container?: boolean;
    damageDice?: number;
    damageType?: any;
    description?: string;
    diceDrop?: number;
    enchantment?: string;
    god?: string;
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

    static fromJson(jsonData: IItemTemplateData): ItemTemplateData {
        let itemTemplateData = new ItemTemplateData();
        Object.assign(itemTemplateData, jsonData, {gun: ItemTemplateGunData.fromResponse(jsonData.gun)});
        return itemTemplateData;
    }

    isItemTypeName(itemTypeName: string): boolean {
        if (!this.itemTypes) {
            return false;
        }
        let i = this.itemTypes.findIndex(name => name === itemTypeName);
        return i !== -1;
    }

    isItemType(itemType: ItemTypeResponse): boolean {
        if (!this.itemTypes) {
            return false;
        }
        let i = this.itemTypes.findIndex(name => name === itemType.techName);
        return i !== -1;
    }

    toggleItemType(itemType: ItemTypeResponse): void {
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
    techName?: string;
    categoryId: number;
    data: ItemTemplateData = new ItemTemplateData();
    source: 'official' | 'community' | 'private';
    sourceUser?: string;
    sourceUserId?: number;
    modifiers: ItemStatModifier[] = [];
    skills: Skill[] = [];
    unskills: Skill[] = [];
    slots: ItemSlot[] = [];
    restrictJobs: Job[] = [];
    requirements: {
        stat: string;
        min?: number;
        max?: number;
    }[] = [];
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

    static fromResponse(response: ItemTemplateResponse, skillsById: SkillDictionary): ItemTemplate {
        let itemTemplate = new ItemTemplate();
        Object.assign(itemTemplate, response, {
            skills: [],
            unskills: [],
            skillModifiers: [],
            data: ItemTemplateData.fromJson(response.data)
        });

        for (let s of response.skills) {
            itemTemplate.skills.push(skillsById[s.id]);
        }
        for (let s of response.unskills) {
            itemTemplate.unskills.push(skillsById[s.id]);
        }

        for (let skillModifier of response.skillModifiers) {
            itemTemplate.skillModifiers.push(new ItemSkillModifier(skillsById[+skillModifier.skill], skillModifier.value));
        }

        return itemTemplate;
    }

    static fromResponses(responses: ItemTemplateResponse[], skillsById: SkillDictionary): ItemTemplate[] {
        return responses.map(response => ItemTemplate.fromResponse(response, skillsById));
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

    addSkill(skill: Skill) {
        if (!this.skills) {
            this.skills = [];
        }
        this.skills.push(skill);
    }

    removeSkill(skill: Skill) {
        if (this.skills) {
            let i = this.skills.findIndex(s => s.id === skill.id);
            if (i !== -1) {
                this.skills.splice(i, 1);
            }
        }
    }

    addUnskill(skill: Skill) {
        if (!this.unskills) {
            this.unskills = [];
        }
        this.unskills.push(skill);
    }

    removeUnskill(skill: Skill) {
        if (this.unskills) {
            let i = this.unskills.findIndex(s => s.id === skill.id);
            if (i !== -1) {
                this.unskills.splice(i, 1);
            }
        }
    }
}
