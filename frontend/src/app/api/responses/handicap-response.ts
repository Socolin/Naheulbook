export type HandicapData = {
    /** What the character must have for this handicap to apply (origin, skill...). Absent when it applies to everyone. */
    condition?: string;
    effect: string;
}

export type HandicapResponse = {
    id: string;
    /** Result of the D100 roll that gives this handicap */
    diceRoll: number;
    name: string;
    data: HandicapData;
}

export type HandicapsResponse = {
    handicaps: HandicapResponse[];
}
