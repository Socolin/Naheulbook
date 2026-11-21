/** An entry of the Mankdebol calamities or Débilibeuk blessings D100 table */
export type EntropyEventResponse = {
    id: string;
    /** Result of the D100 roll that gives this entry */
    diceRoll: number;
    name: string;
    effect: string;
}

export type EntropyTablesResponse = {
    mankdebolCalamities: EntropyEventResponse[];
    debilibeukBlessings: EntropyEventResponse[];
}
