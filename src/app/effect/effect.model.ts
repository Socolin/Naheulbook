import {StatModifier} from '../shared';
import {DurationType, IDurable} from '../date/durable.model';
import {isNullOrUndefined} from 'util';

export class EffectCategory {
    id: number;
    name: string;
    diceSize: number;
    diceCount: number;
    note: string;

    static fromJson(jsonData: any): EffectCategory {
        let effectCategory = new EffectCategory();
        Object.assign(effectCategory, jsonData);
        return effectCategory;
    }

    static categoriesFromJson(jsonDatas: any[]): EffectCategory[] {
        let effectCategories = [];
        for (let categoryData of jsonDatas) {
            effectCategories.push(EffectCategory.fromJson(categoryData));
        }
        return effectCategories;
    }
}

export interface EffectJsonData {
    id: number;
    name: string;
    category: number;
    description: string;
    modifiers: StatModifier[];
    dice: number;
    durationType: DurationType;
    combatCount: number;
    lapCount: number;
    duration: string;
    timeDuration: number;
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
        Object.assign(effect, effectJsonData);
        effect.category = categoriesById[effectJsonData.category];
        return effect;
    }

    static effectsFromJson(categoriesById: {[categoryId: number]: EffectCategory}, effectsJsonData: EffectJsonData[]): Effect[] {
        let effects = [];
        for (let effectJsonData of effectsJsonData) {
            effects.push(Effect.fromJson(effectJsonData, categoriesById));
        }
        return effects;
    }

    get categoryId() {
        if (isNullOrUndefined(this.category)) {
            return 0;
        }
        return this.category.id;
    }
}

