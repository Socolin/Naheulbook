export type StatModificationOperand =
    'ADD'
    | 'MUL'
    | 'DIV'
    | 'SET'
    | 'PERCENTAGE';

export class ItemStatModifier {
    stat: string;
    type: StatModificationOperand;
    value: number;
    special: string[];
    job: number;
    origin: number;
}

export interface StatModifier {
    stat: string;
    type: StatModificationOperand;
    value: number;
    special?: string[];
}
