import {ChangeDetectionStrategy, Component, Input, OnDestroy, OnInit} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {firstValueFrom, Subscription} from 'rxjs';
import {MatFormField, MatLabel, MatPrefix, MatSuffix} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatIcon} from '@angular/material/icon';
import {MatButton, MatIconButton} from '@angular/material/button';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {MatTooltip} from '@angular/material/tooltip';
import {SpellBookResponse, SpellResponse} from '../api/responses';
import {Character} from '../character/character.model';
import {NhbkMatDialog} from '../material-workaround';
import {SpellService} from './spell.service';
import {SpellCardComponent} from './spell-card.component';
import {normalizeForSearch, spellSearchText} from './spell-view';
import {CharacterSpellBook, getCharacterSpellBooks, SPELL_BOOK_SOURCE_LABELS} from './character-spells';
import {
    CharacterSpellBookDialogComponent,
    CharacterSpellBookDialogData,
    CharacterSpellBookDialogResult
} from './character-spell-book-dialog.component';

interface SpellLevelGroup {
    label: string;
    spells: SpellResponse[];
}

/**
 * Spells available to a character: books of its specialities, generalist magic, and the books added to it
 * (special events, rare books…), limited to the spells of its level.
 */
@Component({
    selector: 'character-spell-panel',
    imports: [
        FormsModule, MatFormField, MatLabel, MatPrefix, MatSuffix, MatInput, MatIcon, MatButton, MatIconButton,
        MatProgressSpinner, MatTooltip, SpellCardComponent,
    ],
    templateUrl: './character-spell-panel.component.html',
    styleUrl: './character-spell-panel.component.scss',
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class CharacterSpellPanelComponent implements OnInit, OnDestroy {
    @Input({required: true}) character: Character;

    readonly sourceLabels = SPELL_BOOK_SOURCE_LABELS;
    allBooks?: SpellBookResponse[];
    filter = '';

    private selectedBookId?: string;
    private cacheKey?: string;
    private cachedBooks: CharacterSpellBook[] = [];
    private filterCacheKey?: string;
    private cachedLevelGroups: SpellLevelGroup[] = [];
    private readonly searchTexts = new Map<string, string>();
    private readonly subscription = new Subscription();

    constructor(
        private readonly spellService: SpellService,
        private readonly dialog: NhbkMatDialog,
    ) {
    }

    ngOnInit() {
        this.subscription.add(this.spellService.getSpellBooks().subscribe(books => {
            this.allBooks = books;
        }));
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }

    /** Recomputed only when something used by the rules changes (level up, speciality, added book…) */
    get books(): CharacterSpellBook[] {
        if (!this.allBooks) {
            return [];
        }
        const input = {
            level: this.character.level,
            jobIds: this.character.jobs.map(j => j.id),
            specialityIds: this.character.specialities.map(s => s.id),
            spellBooks: this.character.spellBooks,
        };
        const key = JSON.stringify(input);
        if (key !== this.cacheKey) {
            this.cacheKey = key;
            this.cachedBooks = getCharacterSpellBooks(this.allBooks, input);
        }
        return this.cachedBooks;
    }

    get selectedBook(): CharacterSpellBook | undefined {
        const books = this.books;
        return books.find(b => b.book.id === this.selectedBookId) ?? books[0];
    }

    get levelGroups(): SpellLevelGroup[] {
        const selected = this.selectedBook;
        if (!selected) {
            return [];
        }
        const key = selected.book.id + '|' + selected.maxLevel + '|' + this.filter;
        if (key !== this.filterCacheKey || !this.cachedBooks.includes(selected)) {
            this.filterCacheKey = key;
            const terms = normalizeForSearch(this.filter).split(/\s+/).filter(t => t);
            const spells = selected.spells.filter(spell => terms.every(t => this.getSearchText(spell).includes(t)));
            this.cachedLevelGroups = this.groupByLevel(spells);
        }
        return this.cachedLevelGroups;
    }

    selectBook(view: CharacterSpellBook) {
        this.selectedBookId = view.book.id;
    }

    countVisible(groups: SpellLevelGroup[]): number {
        return groups.reduce((sum, g) => sum + g.spells.length, 0);
    }

    async openAddDialog() {
        const books = this.allBooks ?? await firstValueFrom(this.spellService.getSpellBooks());
        const addedIds = new Set(this.character.spellBooks.map(x => x.spellBookId));
        const result = await this.openDialog({books: books.filter(b => !addedIds.has(b.id))});
        if (!result) {
            return;
        }
        const response = await firstValueFrom(this.spellService.addCharacterSpellBook(this.character.id, result));
        this.character.addOrUpdateSpellBook(response);
        this.selectedBookId = response.spellBookId;
    }

    async openEditDialog(view: CharacterSpellBook) {
        if (!view.link) {
            return;
        }
        const result = await this.openDialog({books: [], editedBook: view.book, maxLevel: view.link.maxLevel});
        if (!result) {
            return;
        }
        const response = await firstValueFrom(this.spellService.updateCharacterSpellBook(this.character.id, view.book.id, {maxLevel: result.maxLevel}));
        this.character.addOrUpdateSpellBook(response);
    }

    remove(view: CharacterSpellBook) {
        if (!confirm(`Retirer le grimoire « ${view.book.name} » de ${this.character.name} ?`)) {
            return;
        }
        this.spellService.removeCharacterSpellBook(this.character.id, view.book.id)
            .subscribe(() => this.character.removeSpellBook(view.book.id));
    }

    clearFilter() {
        this.filter = '';
    }

    private openDialog(data: CharacterSpellBookDialogData): Promise<CharacterSpellBookDialogResult | undefined> {
        const dialogRef = this.dialog.open<CharacterSpellBookDialogComponent, CharacterSpellBookDialogData, CharacterSpellBookDialogResult>(
            CharacterSpellBookDialogComponent,
            {data}
        );
        return firstValueFrom(dialogRef.afterClosed());
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
