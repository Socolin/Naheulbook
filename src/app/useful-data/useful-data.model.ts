export type PanelNames =
    'criticalSuccess'
    | 'epicFails'
    | 'effects'
    | 'entropicSpells'
    | 'recovery'
    | 'travel'
    | 'skills'
    | 'items'
    | 'jobs'
    | 'origins'
    | 'monsters';

export class CriticalData {
    dice: number[];
    effect: string;
    effect2: string;
}
