export type AddCharacterTalentRequest = {
    talentId: string;
}

export type AddCharacterMasteryRequest = {
    masteryId: string;
}

export type AddCharacterHandicapRequest = {
    handicapId: string;
}

export type AddCharacterSpellBookRequest = {
    spellBookId: string;
    maxLevel?: number | null;
}

export type UpdateCharacterSpellBookRequest = {
    maxLevel?: number | null;
}
