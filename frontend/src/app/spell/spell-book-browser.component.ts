import {ChangeDetectionStrategy, Component, ContentChild, EventEmitter, Input, Output, TemplateRef} from '@angular/core';
import {NgTemplateOutlet} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MatFormField, MatLabel, MatPrefix, MatSuffix} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatIcon} from '@angular/material/icon';
import {MatIconButton} from '@angular/material/button';
import {SpellBookResponse, SpellResponse} from '../api/responses';
import {SpellCardComponent} from './spell-card.component';
import {normalizeForSearch, spellSearchText} from './spell-view';

/** A spell book with the spells available to someone (character, monster…) */
export interface SpellBookEntry {
    book: SpellBookResponse;
    /** Available spells, in the book order */
    spells: SpellResponse[];
    /** Highest spell level available. Absent: no limit */
    maxLevel?: number;
    /** Number of spells of the book not available (level too high) */
    lockedCount: number;
}

export interface SpellBookEntryTemplateContext<T extends SpellBookEntry> {
    $implicit: T;
}

interface SpellLevelGroup {
    label: string;
    spells: SpellResponse[];
}

/**
 * List of spell books, with a search field, displaying the spells of the selected book grouped by level.
 *
 * Content:
 * - `<ng-template #bookMeta let-entry>`: displayed under the book title
 * - `<ng-template #bookActions let-entry>`: buttons displayed next to the book title
 * - default content: displayed next to the search field (actions)
 * - `[empty]` content: displayed when there is no book
 */
@Component({
    selector: 'spell-book-browser',
    imports: [
        NgTemplateOutlet, FormsModule, MatFormField, MatLabel, MatPrefix, MatSuffix, MatInput, MatIcon, MatIconButton,
        SpellCardComponent,
    ],
    templateUrl: './spell-book-browser.component.html',
    styleUrl: './spell-book-browser.component.scss',
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class SpellBookBrowserComponent<T extends SpellBookEntry = SpellBookEntry> {
    @Input({required: true}) books: T[] = [];
    @Input() selectedBookId?: string;
    @Output() selectedBookIdChange = new EventEmitter<string>();

    @ContentChild('bookMeta') bookMetaTemplate?: TemplateRef<SpellBookEntryTemplateContext<T>>;
    @ContentChild('bookActions') bookActionsTemplate?: TemplateRef<SpellBookEntryTemplateContext<T>>;

    filter = '';

    private cacheKey?: string;
    private cachedEntry?: T;
    private cachedLevelGroups: SpellLevelGroup[] = [];
    private readonly searchTexts = new Map<string, string>();

    get selectedBook(): T | undefined {
        return this.books.find(b => b.book.id === this.selectedBookId) ?? this.books[0];
    }

    get levelGroups(): SpellLevelGroup[] {
        const selected = this.selectedBook;
        if (!selected) {
            return [];
        }
        const key = selected.book.id + '|' + this.filter;
        if (key !== this.cacheKey || selected !== this.cachedEntry) {
            this.cacheKey = key;
            this.cachedEntry = selected;
            const terms = normalizeForSearch(this.filter).split(/\s+/).filter(t => t);
            const spells = selected.spells.filter(spell => terms.every(t => this.getSearchText(spell).includes(t)));
            this.cachedLevelGroups = this.groupByLevel(spells);
        }
        return this.cachedLevelGroups;
    }

    selectBook(entry: T) {
        this.selectedBookId = entry.book.id;
        this.selectedBookIdChange.emit(entry.book.id);
    }

    countVisible(groups: SpellLevelGroup[]): number {
        return groups.reduce((sum, g) => sum + g.spells.length, 0);
    }

    clearFilter() {
        this.filter = '';
    }

    private getSearchText(spell: SpellResponse): string {
        let text = this.searchTexts.get(spell.id);
        if (text === undefined) {
            text = spellSearchText(spell);
            this.searchTexts.set(spell.id, text);
        }
        return text;
    }

    /** Group the spells by level label ("Niveau 1", "Multi-niveaux"…), keeping the order of the book */
    private groupByLevel(spells: SpellResponse[]): SpellLevelGroup[] {
        const groups: SpellLevelGroup[] = [];
        for (const spell of spells) {
            const label = spell.levelLabel ?? 'Sans niveau';
            let group = groups.find(g => g.label === label);
            if (!group) {
                group = {label, spells: []};
                groups.push(group);
            }
            group.spells.push(spell);
        }
        return groups;
    }
}
