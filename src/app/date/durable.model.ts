export type DurationType = 'custom' | 'time' | 'combat' | 'lap' | 'forever';

export interface IDurable {
    durationType: DurationType;
    combatCount?: number;
    lapCount?: number;
    duration?: string;
    timeDuration?: number;
}
