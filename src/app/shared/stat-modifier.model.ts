import {DurationType, IDurable} from '../date/durable.model';

export type StatModificationOperand =
    'ADD'
    | 'MUL'
    | 'DIV'
    | 'SET'
    | 'PERCENTAGE';

export interface StatModifier {
    stat: string;
    type: StatModificationOperand;
    value: number;
    special?: string[];
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

    reusable: boolean = false;

    durationType: DurationType = 'combat';
    duration: string;
    combatCount: number = 1;
    lapCount: number = 1;
    timeDuration: number;

    values: StatModifier[] = [];
}
