import {ChangeDetectionStrategy, Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {FormsModule} from '@angular/forms';
import {Subscription} from 'rxjs';
import {MatFormField, MatLabel, MatPrefix, MatSuffix} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatIcon} from '@angular/material/icon';
import {MatIconButton} from '@angular/material/button';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {SpellBookKind, SpellBookResponse, SpellResponse} from '../api/responses';
import {SpellService} from './spell.service';
import {normalizeForSearch, SPELL_BOOK_KIND_LABELS, spellSearchText} from './spell-view';
import {SpellCardComponent} from './spell-card.component';

interface SpellItem {
    spell: SpellResponse;
    searchText: string;
}

interface SpellLevelGroup {
    label: string;
    spells: SpellResponse[];
}

interface SpellBookView {
    book: SpellBookResponse;
    spells: SpellItem[];
    visibleCount: number;
    levelGroups: SpellLevelGroup[];
}

interface SpellBookKindGroup {
    kind: SpellBookKind;
    label: string;
    books: SpellBookView[];
}

@Component({
    selector: 'app-spells',
    imports: [
        FormsModule,
        MatFormField,
        MatLabel,
        MatPrefix,
        MatSuffix,
        MatInput,
        MatIcon,
        MatIconButton,
        MatProgressSpinner,
        SpellCardComponent,
    ],
    templateUrl: './spells.component.html',
    styleUrl: './spells.component.scss',
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class SpellsComponent implements OnInit, OnDestroy {
    kindGroups?: SpellBookKindGroup[];
    selectedBook?: SpellBookView;
    filter = '';

    private books: SpellBookView[] = [];
    private selectedTechName?: string;
    private readonly subscription = new Subscription();

    constructor(
        private readonly spellService: SpellService,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
    ) {
    }

    ngOnInit() {
        this.subscription.add(this.route.params.subscribe(params => {
            this.selectedTechName = params['spellBookTechName'];
            this.updateSelectedBook();
        }));
        this.subscription.add(this.spellService.getSpellBooks().subscribe(books => {
            this.books = books.map(book => ({
                book,
                spells: book.spells.map(spell => ({spell, searchText: spellSearchText(spell)})),
                visibleCount: book.spells.length,
                levelGroups: [],
            }));
            const kindGroups: SpellBookKindGroup[] = [];
            for (const view of this.books) {
                let group = kindGroups.find(g => g.kind === view.book.kind);
                if (!group) {
                    group = {kind: view.book.kind, label: SPELL_BOOK_KIND_LABELS[view.book.kind] ?? view.book.kind, books: []};
                    kindGroups.push(group);
                }
                group.books.push(view);
            }
            this.kindGroups = kindGroups;
            this.updateFilter();
            this.updateSelectedBook();
        }));
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }

    selectBook(view: SpellBookView) {
        if (view === this.selectedBook) {
            return;
        }
        this.selectedBook = view;
        this.router.navigate(['/database/spells', view.book.techName], {replaceUrl: true});
    }

    updateFilter() {
        const terms = normalizeForSearch(this.filter).split(/\s+/).filter(t => t);
        for (const view of this.books) {
            const visibleSpells = view.spells
                .filter(item => terms.every(t => item.searchText.includes(t)))
                .map(item => item.spell);
            view.visibleCount = visibleSpells.length;
            view.levelGroups = this.groupByLevel(visibleSpells);
        }
    }

    clearFilter() {
        this.filter = '';
        this.updateFilter();
    }

    /**
     * Group the spells by level label ("Niveau 1", "Multi-niveaux", "Expert"…), keeping the order of the book
     */
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

    private updateSelectedBook() {
        if (!this.books.length) {
            return;
        }
        this.selectedBook = this.books.find(b => b.book.techName === this.selectedTechName) ?? this.books[0];
    }
}
