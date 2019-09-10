export interface IStatModifier {
    stat: string;
    type: 'ADD'
        | 'MUL'
        | 'DIV'
        | 'SET'
        | 'PERCENTAGE';
    value: number;
    special?: string[];
}
