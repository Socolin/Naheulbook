import {DurationType, IDurable} from '../date/durable.model';
import {Effect} from '../effect/effect.model';

export type StatModificationOperand =
    'ADD'
    | 'MUL'
    | 'DIV'
    | 'SET'
    | 'PERCENTAGE';

export class StatModifier {
    stat: string;
    type: StatModificationOperand;
    value: number;
    special?: string[];

    static apply(value: number, mod: StatModifier) {
        if (mod.type === 'ADD') {
            return value + mod.value;
        }
        else if (mod.type === 'SET') {
            return mod.value;
        }
        else if (mod.type === 'DIV') {
            return value / mod.value;
        }
        else if (mod.type === 'MUL') {
            return value * mod.value;
        }
        else if (mod.type === 'PERCENTAGE') {
            return value * (mod.value / 100);
        }
        throw new Error('Invalid stat modifier')
    }
}

export class ItemStatModifier implements StatModifier {
    stat: string;
    type: StatModificationOperand;
    value: number;
    special: string[];

    job: number;
    origin: number;
}

export class StatsModifier implements IDurable {
    name: string;

    reusable = false;

    durationType: DurationType = 'combat';
    duration: string;
    combatCount = 1;
    lapCount = 1;
    timeDuration: number;

    description?: string;
    type?: string;

    values: StatModifier[] = [];
}

export class ActiveStatsModifier extends StatsModifier {
    id: number;
    permanent: boolean;
    active: boolean;

    currentCombatCount: number;
    currentLapCount: number;
    currentTimeDuration: number;

    static fromJson(jsonData: any) {
        let modifier = new ActiveStatsModifier();
        Object.assign(modifier, jsonData);
        return modifier;
    }

    static modifiersFromJson(modifiersJsonData: undefined|null|any[]) {
        let modifiers = [];

        if (modifiersJsonData) {
            for (let modifierJsonData of modifiersJsonData) {
                modifiers.push(ActiveStatsModifier.fromJson(modifierJsonData));
            }
        }

        return modifiers;
    }

    static fromEffect(effect: Effect, data: any): ActiveStatsModifier {
        let modifier = new ActiveStatsModifier();
        Object.assign(modifier, effect);
        modifier.name = effect.name;
        modifier.description = effect.description;
        modifier.permanent = false;
        modifier.reusable = data.reusable;
        modifier.type = effect.category.name;
        if ('durationType'  in data) {
            modifier.durationType = data.durationType;
            switch (data.durationType) {
                case 'combat':
                    modifier.combatCount = data.combatCount;
                    modifier.currentCombatCount = data.combatCount;
                    break;
                case 'time':
                    modifier.timeDuration = data.timeDuration;
                    modifier.currentTimeDuration = data.timeDuration;
                    break;
                case 'lap':
                    modifier.lapCount = data.lapCount;
                    modifier.currentLapCount = data.lapCount;
                    break;
                case 'custom':
                    modifier.duration = data.duration;
                    break;
                case 'forever':
                    break;
            }
        } else {
            modifier.durationType = effect.durationType;
            modifier.combatCount = effect.combatCount;
            modifier.currentCombatCount = effect.combatCount;
            modifier.lapCount = effect.lapCount;
            modifier.currentLapCount = effect.lapCount;
            modifier.timeDuration = effect.timeDuration;
            modifier.currentTimeDuration = effect.timeDuration;
            modifier.duration = effect.duration;
        }
        if (effect.modifiers) {
            modifier.values = JSON.parse(JSON.stringify(effect.modifiers));
        }
        return modifier;
    }
}
