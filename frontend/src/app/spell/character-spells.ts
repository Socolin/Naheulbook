import {CharacterSpellBookResponse, SpellBookResponse, SpellResponse} from '../api/responses';

export const GENERALIST_SPELL_BOOK_TECH_NAME = 'mage-generaliste';
export const INVOCATION_SPELL_BOOK_TECH_NAME = 'mage-invocation';

/** Why a character has access to a spell book */
export type CharacterSpellBookSource = 'speciality' | 'generalist' | 'added';

export interface CharacterSpellBook {
    book: SpellBookResponse;
    sources: CharacterSpellBookSource[];
    /** Set when the book was added to the character (special event, rare book…) */
    link?: CharacterSpellBookResponse;
    /** Highest spell level available */
    maxLevel: number;
    /** Available spells, in the book order */
    spells: SpellResponse[];
    /** Number of spells of the book not available yet (level too high) */
    lockedCount: number;
}

export interface CharacterSpellsInput {
    level: number;
    jobIds: string[];
    specialityIds: string[];
    spellBooks: CharacterSpellBookResponse[];
}

/**
 * Spell books available to a character, following the rules of the books ("regles-magie-pretrise"):
 * - The books of its specialities (mage speciality, god of a priest or paladin)
 * - The generalist magic for mages, except for invocateurs
 * - The books added to the character (special event like the "Mutant pyromane" bienfait, rare book…),
 *   optionally limited to a max level
 *
 * A spell of level N is available from the character level N. Spells without level (multi-level disciplines,
 * forbidden invocations…) are always available.
 */
export function getCharacterSpellBooks(books: SpellBookResponse[], character: CharacterSpellsInput): CharacterSpellBook[] {
    const specialityIds = new Set(character.specialityIds);
    const jobIds = new Set(character.jobIds);
    const invocationSpecialityId = books.find(b => b.techName === INVOCATION_SPELL_BOOK_TECH_NAME)?.specialityId;
    const isInvocateur = !!invocationSpecialityId && specialityIds.has(invocationSpecialityId);

    const result: CharacterSpellBook[] = [];
    for (const book of books) {
        const sources: CharacterSpellBookSource[] = [];
        if (book.specialityId && specialityIds.has(book.specialityId)) {
            sources.push('speciality');
        }
        if (book.techName === GENERALIST_SPELL_BOOK_TECH_NAME && book.jobId && jobIds.has(book.jobId) && !isInvocateur) {
            sources.push('generalist');
        }
        const link = character.spellBooks.find(x => x.spellBookId === book.id);
        if (link) {
            sources.push('added');
        }
        if (!sources.length) {
            continue;
        }

        // A max level on an added book only limits that source: the speciality gives the whole book anyway
        const maxLevel = sources.length === 1 && link?.maxLevel
            ? Math.min(character.level, link.maxLevel)
            : character.level;
        const spells = book.spells.filter(s => s.level === undefined || s.level === null || s.level <= maxLevel);
        result.push({
            book,
            sources,
            link,
            maxLevel,
            spells,
            lockedCount: book.spells.length - spells.length,
        });
    }
    return result;
}

export const SPELL_BOOK_SOURCE_LABELS: Record<CharacterSpellBookSource, string> = {
    speciality: 'Spécialité',
    generalist: 'Magie généraliste',
    added: 'Ajouté',
};
