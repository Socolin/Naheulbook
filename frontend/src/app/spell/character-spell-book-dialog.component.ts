import {ChangeDetectionStrategy, Component, Inject} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogActions, MatDialogClose, MatDialogContent, MatDialogRef, MatDialogTitle} from '@angular/material/dialog';
import {MatFormField, MatHint, MatLabel} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatOptgroup, MatOption, MatSelect} from '@angular/material/select';
import {MatButton} from '@angular/material/button';
import {SpellBookKind, SpellBookResponse} from '../api/responses';
import {SPELL_BOOK_KIND_LABELS} from './spell-view';

export interface CharacterSpellBookDialogData {
    /** Books that can be chosen. Ignored when editing */
    books: SpellBookResponse[];
    /** Set when editing the max level of a book already added */
    editedBook?: SpellBookResponse;
    maxLevel?: number | null;
    /** Help text of the max level field */
    maxLevelHint?: string;
}

export interface CharacterSpellBookDialogResult {
    spellBookId: string;
    maxLevel: number | null;
}

@Component({
    templateUrl: './character-spell-book-dialog.component.html',
    styles: `
        .fields {
            display: flex;
            flex-direction: column;
            gap: 8px;
            min-width: min(420px, 80vw);
            padding-top: 8px;
        }
    `,
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [
        FormsModule, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose,
        MatFormField, MatLabel, MatHint, MatInput, MatSelect, MatOption, MatOptgroup, MatButton,
    ],
})
export class CharacterSpellBookDialogComponent {
    spellBookId?: string;
    maxLevel: number | null;
    readonly groups: { label: string, books: SpellBookResponse[] }[] = [];

    constructor(
        private readonly dialogRef: MatDialogRef<CharacterSpellBookDialogComponent, CharacterSpellBookDialogResult>,
        @Inject(MAT_DIALOG_DATA) public readonly data: CharacterSpellBookDialogData,
    ) {
        this.spellBookId = data.editedBook?.id;
        this.maxLevel = data.maxLevel ?? null;
        for (const book of data.books) {
            const label = SPELL_BOOK_KIND_LABELS[book.kind as SpellBookKind] ?? book.kind;
            let group = this.groups.find(g => g.label === label);
            if (!group) {
                group = {label, books: []};
                this.groups.push(group);
            }
            group.books.push(book);
        }
    }

    get isValid(): boolean {
        return !!this.spellBookId && (this.maxLevel === null || (Number.isInteger(this.maxLevel) && this.maxLevel >= 1));
    }

    valid() {
        if (!this.spellBookId || !this.isValid) {
            return;
        }
        this.dialogRef.close({spellBookId: this.spellBookId, maxLevel: this.maxLevel || null});
    }
}
