import {ChangeDetectionStrategy, Component, Inject, OnDestroy, OnInit} from '@angular/core';
import {Subscription} from 'rxjs';
import {MAT_DIALOG_DATA, MatDialogClose} from '@angular/material/dialog';
import {MatToolbar} from '@angular/material/toolbar';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {MonsterSpellBookData} from '../api/shared';
import {SpellService} from './spell.service';
import {SpellBookBrowserComponent} from './spell-book-browser.component';
import {getMonsterSpellBooks, MonsterSpellBook} from './monster-spells';

export interface MonsterSpellsDialogData {
    name: string;
    spellBooks: MonsterSpellBookData[];
}

/** Spells a monster can cast, from the spell books of its data */
@Component({
    templateUrl: './monster-spells-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './monster-spells-dialog.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon, MatProgressSpinner, SpellBookBrowserComponent],
})
export class MonsterSpellsDialogComponent implements OnInit, OnDestroy {
    books?: MonsterSpellBook[];
    private readonly subscription = new Subscription();

    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: MonsterSpellsDialogData,
        private readonly spellService: SpellService,
    ) {
    }

    ngOnInit() {
        this.subscription.add(this.spellService.getSpellBooks().subscribe(books => {
            this.books = getMonsterSpellBooks(books, this.data.spellBooks);
        }));
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }
}
