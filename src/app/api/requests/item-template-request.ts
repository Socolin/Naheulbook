import {IItemTemplateData} from '../shared';

export interface ItemTemplateRequest
{
    source: 'official'|'community'|'private';
    categoryId: number;
    name: string;
    techName?: string;
    modifiers: ItemTemplateModifierRequest[];
    skills: {id: number}[];
    unskills: {id: number}[];
    skillModifiers: ItemTemplateSkillModifierRequest[];
    requirements: ItemTemplateRequirementRequest[];
    slots: {id: number}[];
    data: IItemTemplateData;
}

export interface ItemTemplateModifierRequest
{
    stat: string;
    value: number;
    type: string;
    special: string[];
    job: number | null;
    origin: number | null;
}

export interface ItemTemplateSkillModifierRequest
{
    skill: number;
    value: number;
}

export interface ItemTemplateRequirementRequest
{
    stat: string;
    min: number;
    max: number;
}
