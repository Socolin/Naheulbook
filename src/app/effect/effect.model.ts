import {StatModifier} from '../shared';

export class EffectCategory {
    id: number;
    name: string;
    diceSize: number;
    diceCount: number;
    nore: string;
}

export class Effect {
    id: number;
    name: string;
    description: string;
    dice: number;
    combatCount: number;
    lapCount: number;
    modifiers: StatModifier[] = [];
    duration: string;
    category: number;
    timeDuration: number;
}
