export interface CreateEffectCategoryRequest {
    name: string;
    diceCount: number;
    diceSize: number;
    note?: string;
    typeId: number;
}
