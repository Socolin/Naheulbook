export interface JobStatData {
    readonly maxLoad?: number;
    readonly maxArmorPr?: number;
    /** Number of special attacks a character can know = level + specialAttackLimitBonus (1 when missing) */
    readonly specialAttackLimitBonus?: number;
    readonly bonusOriginMaxArmorPr?: number;
    readonly factorEv?: number;
    readonly baseEv?: number;
    readonly bonusEv?: number;
    readonly baseEa?: number;
    readonly diceEaLevelUp?: number;
    readonly baseAt?: number;
    readonly basePrd?: number;
}
