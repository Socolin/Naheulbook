import {IItemTemplateData} from '../shared';
import {ItemSlotResponse} from './item-slot.response';

export interface ItemTemplateResponse {
    id: number;
    categoryId: number;
    name: string;
    techName?: string;
    source: string;
    sourceUserId?: number;
    sourceUser?: string;
    data: IItemTemplateData;

    modifiers: ItemTemplateModifierResponse[];
    skills: { id: number }[];
    unskills: { id: number }[];
    skillModifiers: ItemTemplateSkillModifierResponse[];
    requirements: ItemTemplateRequirementResponse[];
    slots: ItemSlotResponse[];
}

export interface ItemTemplateModifierResponse {
    stat: string;
    value: number;
    type: string;
    special?: string[];
    jobId?: number;
    originId?: number;
}

export interface ItemTemplateSkillModifierResponse {
    skill: number;
    value: number;
}

export interface ItemTemplateRequirementResponse {
    stat: string;
    min: number;
    max: number;
}
