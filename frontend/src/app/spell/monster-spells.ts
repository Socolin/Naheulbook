import {SpellBookResponse} from '../api/responses';
import {MonsterSpellBookData} from '../api/shared';
import {SpellBookEntry} from './spell-book-browser.component';

export interface MonsterSpellBook extends SpellBookEntry {
    link: MonsterSpellBookData;
}

/**
 * Spell books of a monster, in the order of the books. A spell of level N is available when N <= maxLevel
 * (no maxLevel: the whole book). Spells without level are always available. Unknown books are ignored.
 */
export function getMonsterSpellBooks(books: SpellBookResponse[], links: MonsterSpellBookData[] | undefined): MonsterSpellBook[] {
    const result: MonsterSpellBook[] = [];
    for (const book of books) {
        const link = links?.find(x => x.spellBookId === book.id);
        if (!link) {
            continue;
        }
        const maxLevel = link.maxLevel || undefined;
        const spells = book.spells.filter(s => s.level === undefined || s.level === null || !maxLevel || s.level <= maxLevel);
        result.push({book, link, maxLevel, spells, lockedCount: book.spells.length - spells.length});
    }
    return result;
}

/** "Magie Généraliste (niv. 1 à 5)" */
export function formatMonsterSpellBook(entry: MonsterSpellBook): string {
    return entry.maxLevel ? `${entry.book.name} (niv. 1 à ${entry.maxLevel})` : entry.book.name;
}
