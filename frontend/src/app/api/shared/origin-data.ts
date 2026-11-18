export interface OriginData {
    readonly baseEv: number;
    readonly diceEvLevelUp: number;
    readonly bonusAt?: number;
    readonly bonusPrd?: number;
    readonly maxArmorPr?: number;
    readonly maxLoad?: number;
    readonly speedModifier?: number;
    readonly creationStatModifiers?: { [statName: string]: number };
    readonly alternativeAptitudeGroupIds?: string[];
}
