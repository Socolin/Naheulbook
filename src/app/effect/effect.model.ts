import {StatModifier} from '../shared';
import {DurationType, IDurable} from '../date/durable.model';

export class EffectCategory {
    id: number;
    name: string;
    diceSize: number;
    diceCount: number;
    nore: string;
}

export class Effect implements IDurable {
    id: number;
    name: string;
    category: number;
    description: string;
    modifiers: StatModifier[] = [];
    dice: number;
    durationType: DurationType;
    combatCount: number;
    lapCount: number;
    duration: string;
    timeDuration: number;
}
