import {ChangeDetectionStrategy, Component, Input, OnChanges, OnDestroy} from '@angular/core';
import {Subscription} from 'rxjs';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {SpellBookResponse} from '../api/responses';
import {MonsterSpellBookData} from '../api/shared';
import {NhbkMatDialog} from '../material-workaround';
import {SpellService} from './spell.service';
import {formatMonsterSpellBook, getMonsterSpellBooks} from './monster-spells';
import {MonsterSpellsDialogComponent, MonsterSpellsDialogData} from './monster-spells-dialog.component';

/** Spell books of a monster (in the fight panel), with a button to display its spells */
@Component({
    selector: 'monster-spells-summary',
    imports: [MatButton, MatIcon],
    template: `
        <span class="label">Sorts :</span>
        <span class="books">{{ summary }}</span>
        <button mat-button type="button" (click)="openSpellsDialog()">
            <mat-icon fontSet="game-icon" fontIcon="game-icon-spell-book" class="ra-mat-button-icon-24"></mat-icon>
            Voir les sorts
        </button>
    `,
    styles: `
        :host {
            display: flex;
            flex-wrap: wrap;
            align-items: center;
            gap: 0 8px;
        }
    `,
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class MonsterSpellsSummaryComponent implements OnChanges, OnDestroy {
    @Input({required: true}) name!: string;
    @Input({required: true}) spellBooks!: MonsterSpellBookData[];

    summary = '';
    private books?: SpellBookResponse[];
    private subscription?: Subscription;

    constructor(
        private readonly spellService: SpellService,
        private readonly dialog: NhbkMatDialog,
    ) {
    }

    ngOnChanges() {
        if (this.books) {
            this.updateSummary();
        } else if (!this.subscription) {
            this.subscription = this.spellService.getSpellBooks().subscribe(books => {
                this.books = books;
                this.updateSummary();
            });
        }
    }

    ngOnDestroy() {
        this.subscription?.unsubscribe();
    }

    openSpellsDialog() {
        this.dialog.openFullScreen<MonsterSpellsDialogComponent, MonsterSpellsDialogData>(MonsterSpellsDialogComponent, {
            data: {name: this.name, spellBooks: this.spellBooks},
        });
    }

    private updateSummary() {
        this.summary = getMonsterSpellBooks(this.books ?? [], this.spellBooks).map(formatMonsterSpellBook).join(', ');
    }
}
