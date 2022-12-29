import {ActiveStatsModifier} from './stat-modifier.model';
import {IDurable} from '../api/shared';

export type FighterDurationChanges =
    | IMonsterDurationChanges
    | ICharacterDurationChanges;

export interface IMonsterDurationChanges {
    monsterId: number,
    changes: DurationChange[]
}

export interface ICharacterDurationChanges {
    characterId: number,
    changes: DurationChange[]
}

export type DurationChange =
    IItemModifierDurationChange
    | IItemLifetimeDurationChange
    | IModifierDurationChange
    ;

export interface IItemModifierDurationChange {
    type: 'itemModifier';
    itemId: number;
    modifier: ActiveStatsModifier;
}

export interface IItemLifetimeDurationChange {
    type: 'itemLifetime';
    itemId: number;
    lifetime?: IDurable;
}

export interface IModifierDurationChange {
    type: 'modifier';
    modifier: ActiveStatsModifier;
}
