import {IStatModifier} from '../shared';
import {DurationType} from '../shared/enums';

export interface CreateEffectRequest {
    name: string;
    description: string;
    dice: number | null;
    durationType: DurationType;
    duration: string;
    combatCount: number | null;
    lapCount: number | null;
    timeDuration: number | null;
    modifiers: IStatModifier[];
}
