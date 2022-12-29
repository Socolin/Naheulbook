export type GodByTechName = { [techName: string]: God };

export class God {
    id: number;
    displayName: string;
    techName: string;
    description: string;
}
