import {ChangeDetectionStrategy, Component, Input, OnDestroy, OnInit} from '@angular/core';
import {AbstractControl} from '@angular/forms';
import {firstValueFrom, Subscription} from 'rxjs';
import {MatButton, MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatTooltip} from '@angular/material/tooltip';
import {SpellBookResponse} from '../api/responses';
import {MonsterSpellBookData} from '../api/shared';
import {NhbkMatDialog} from '../material-workaround';
import {SpellService} from './spell.service';
import {
    CharacterSpellBookDialogComponent,
    CharacterSpellBookDialogData,
    CharacterSpellBookDialogResult
} from './character-spell-book-dialog.component';

const MAX_LEVEL_HINT = 'Vide : tous les sorts du grimoire.';

/**
 * Edit the spell books a monster (or a monster template) can cast from.
 * The value of `control` is a `MonsterSpellBookData[]` (null / undefined: none).
 */
@Component({
    selector: 'monster-spell-books-editor',
    imports: [MatButton, MatIconButton, MatIcon, MatTooltip],
    templateUrl: './monster-spell-books-editor.component.html',
    styleUrl: './monster-spell-books-editor.component.scss',
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class MonsterSpellBooksEditorComponent implements OnInit, OnDestroy {
    @Input({required: true}) control!: AbstractControl<MonsterSpellBookData[] | null | undefined>;

    booksById = new Map<string, SpellBookResponse>();
    private allBooks?: SpellBookResponse[];
    private readonly subscription = new Subscription();

    constructor(
        private readonly spellService: SpellService,
        private readonly dialog: NhbkMatDialog,
    ) {
    }

    get spellBooks(): MonsterSpellBookData[] {
        return this.control.value ?? [];
    }

    ngOnInit() {
        this.subscription.add(this.spellService.getSpellBooks().subscribe(books => {
            this.allBooks = books;
            this.booksById = new Map(books.map(b => [b.id, b]));
        }));
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }

    async openAddDialog() {
        const books = this.allBooks ?? await firstValueFrom(this.spellService.getSpellBooks());
        const addedIds = new Set(this.spellBooks.map(x => x.spellBookId));
        const result = await this.openDialog({books: books.filter(b => !addedIds.has(b.id)), maxLevelHint: MAX_LEVEL_HINT});
        if (!result) {
            return;
        }
        this.setValue([...this.spellBooks, this.toData(result)]);
    }

    async openEditDialog(spellBook: MonsterSpellBookData) {
        const book = this.booksById.get(spellBook.spellBookId);
        if (!book) {
            return;
        }
        const result = await this.openDialog({books: [], editedBook: book, maxLevel: spellBook.maxLevel, maxLevelHint: MAX_LEVEL_HINT});
        if (!result) {
            return;
        }
        this.setValue(this.spellBooks.map(x => x === spellBook ? this.toData(result) : x));
    }

    remove(spellBook: MonsterSpellBookData) {
        this.setValue(this.spellBooks.filter(x => x !== spellBook));
    }

    private toData(result: CharacterSpellBookDialogResult): MonsterSpellBookData {
        return result.maxLevel ? {spellBookId: result.spellBookId, maxLevel: result.maxLevel} : {spellBookId: result.spellBookId};
    }

    private setValue(spellBooks: MonsterSpellBookData[]) {
        this.control.setValue(spellBooks.length ? spellBooks : undefined);
        this.control.markAsDirty();
    }

    private openDialog(data: CharacterSpellBookDialogData): Promise<CharacterSpellBookDialogResult | undefined> {
        const dialogRef = this.dialog.open<CharacterSpellBookDialogComponent, CharacterSpellBookDialogData, CharacterSpellBookDialogResult>(
            CharacterSpellBookDialogComponent,
            {data}
        );
        return firstValueFrom(dialogRef.afterClosed());
    }
}
