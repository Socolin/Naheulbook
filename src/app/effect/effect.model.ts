import {StatModifier} from '../shared';
import {DurationType, IDurable} from '../date/durable.model';
import {isNullOrUndefined} from 'util';

export interface EffectJsonData {
    id: number;
    name: string;
    categoryId: number;
    description: string;
    modifiers: StatModifier[];
    dice: number;
    durationType: DurationType;
    combatCount: number;
    lapCount: number;
    duration: string;
    timeDuration: number;
}

export class EffectCategory {
    id: number;
    name: string;
    type: EffectType;
    diceSize: number;
    diceCount: number;
    note: string;

    static fromJson(jsonData: any, type: {[id: number]: EffectType}|EffectType): EffectCategory {
        let category = new EffectCategory();
        if (type instanceof  EffectType) {
            Object.assign(category, jsonData, {type: type});
        } else {
            Object.assign(category, jsonData, {type: type[jsonData.typeId]});
        }
        return category;
    }

    static categoriesFromJson(jsonDatas: any[], type: EffectType): EffectCategory[] {
        let categories: EffectCategory[] = [];

        for (let jsonData of jsonDatas) {
            categories.push(EffectCategory.fromJson(jsonData, type));
        }

        return categories;
    }
}

export class EffectType {
    id: number;
    name: string;
    categories: EffectCategory[] = [];

    static fromJson(jsonData: any): EffectType {
        let type = new EffectType();
        Object.assign(type, jsonData, {categories: EffectCategory.categoriesFromJson(jsonData.categories, type)});
        return type;
    }

    static typesFromJson(jsonDatas: any[]): EffectType[] {
        let types: EffectType[] = [];
        for (let jsonData of jsonDatas) {
            types.push(EffectType.fromJson(jsonData));
        }
        return types;
    }
}

export class Effect implements IDurable {
    id: number;
    name: string;
    category: EffectCategory;
    description: string;
    modifiers: StatModifier[] = [];
    dice: number;
    durationType: DurationType = 'forever';
    combatCount: number;
    lapCount: number;
    duration: string;
    timeDuration: number;

    static fromJson(effectJsonData: EffectJsonData, categoriesById: { [categoryId: number]: EffectCategory }): Effect {
        let effect = new Effect();
        Object.assign(effect, effectJsonData, {category: categoriesById[effectJsonData.categoryId]});
        return effect;
    }

    static effectsFromJson(categoriesById: {[categoryId: number]: EffectCategory}, effectsJsonData: EffectJsonData[]): Effect[] {
        let effects: Effect[] = [];
        for (let effectJsonData of effectsJsonData) {
            effects.push(Effect.fromJson(effectJsonData, categoriesById));
        }
        return effects;
    }

    toJsonData(): EffectJsonData {
        let jsonData: any = Object.assign({}, this);
        jsonData.categoryId = this.category.id;
        delete jsonData.categoryId;
        return jsonData;
    }
}

