/** A spell book a monster can cast from, stored in the monster (template) data */
export interface MonsterSpellBookData {
    spellBookId: string;
    /** Only the spells up to this level are available. Absent: the whole book */
    maxLevel?: number | null;
}
