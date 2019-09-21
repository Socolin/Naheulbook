import {StatModifier} from '../shared';
import {IDurable} from '../api/shared';
import {DurationType} from '../api/shared/enums';
import {EffectCategoryResponse, EffectResponse, EffectTypeResponse} from '../api/responses';

export type EffectCategoryDictionary = { [id: number]: EffectCategory };

export class EffectCategory {
    id: number;
    name: string;
    type: EffectType;
    diceSize: number;
    diceCount: number;
    note: string;

    static fromResponse(response: EffectCategoryResponse, type: EffectTypeDictionary | EffectType): EffectCategory {
        let category = new EffectCategory();
        if (type instanceof EffectType) {
            Object.assign(category, response, {type: type});
        } else {
            Object.assign(category, response, {type: type[response.typeId]});
        }
        return category;
    }

    static fromResponses(responses: EffectCategoryResponse[], type: EffectType): EffectCategory[] {
        return responses.map(response => EffectCategory.fromResponse(response, type));
    }
}

export type EffectTypeDictionary = { [id: number]: EffectType };

export class EffectType {
    id: number;
    name: string;
    categories: EffectCategory[] = [];

    static fromResponse(response: EffectTypeResponse): EffectType {
        let type = new EffectType();
        Object.assign(type, response, {categories: EffectCategory.fromResponses(response.categories, type)});
        return type;
    }

    static fromResponses(responses: EffectTypeResponse[]): EffectType[] {
        return responses.map(response => EffectType.fromResponse(response));
    }
}

export class Effect implements IDurable {
    id: number;
    name: string;
    category: EffectCategory;
    description: string;
    modifiers: StatModifier[] = [];
    dice?: number;
    durationType: DurationType = 'forever';
    combatCount?: number;
    lapCount?: number;
    duration?: string;
    timeDuration?: number;

    static fromResponse(response: EffectResponse, categoriesById: EffectCategoryDictionary): Effect {
        let effect = new Effect();
        Object.assign(effect, response, {category: categoriesById[response.categoryId]});
        return effect;
    }

    static fromResponses(categoriesById: EffectCategoryDictionary, responses: EffectResponse[]): Effect[] {
        return responses.map(response => Effect.fromResponse(response, categoriesById));
    }
}

