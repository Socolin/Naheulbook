import {ChangeDetectionStrategy, Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {FormsModule} from '@angular/forms';
import {combineLatest, Subscription} from 'rxjs';
import {MatFormField, MatLabel, MatPrefix, MatSuffix} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatIcon} from '@angular/material/icon';
import {MatIconButton} from '@angular/material/button';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {EntropyEventResponse} from '../api/responses';
import {normalizeEntrySearch} from '../shared/entry-group-tabs/entry-group-tabs.model';
import {EntropyKind, EntropyService} from './entropy.service';

interface EntropyEventItem {
    entry: EntropyEventResponse;
    searchText: string;
}

/**
 * Database page listing the Mankdebol calamities or the Débilibeuk blessings, depending on the `kind` route data
 */
@Component({
    selector: 'app-entropy-table',
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
    ],
    templateUrl: './entropy-table.component.html',
    styleUrls: ['../shared/entry-group-tabs/entry-page.scss', './entropy-table.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class EntropyTableComponent implements OnInit, OnDestroy {
    kind: EntropyKind = 'mankdebol';
    entries?: EntropyEventItem[];
    visibleEntries: EntropyEventItem[] = [];
    filter = '';

    private readonly subscription = new Subscription();

    constructor(
        private readonly route: ActivatedRoute,
        private readonly entropyService: EntropyService,
    ) {
    }

    get title(): string {
        return this.kind === 'mankdebol' ? 'Calamités (Mankdebol)' : 'Bienfaits (Débilibeuk)';
    }

    ngOnInit() {
        this.subscription.add(combineLatest([this.route.data, this.entropyService.getEntropyTables()]).subscribe(([data, tables]) => {
            this.kind = data['kind'] as EntropyKind;
            const entries = this.kind === 'mankdebol' ? tables.mankdebolCalamities : tables.debilibeukBlessings;
            this.entries = entries.map(entry => ({
                entry,
                searchText: normalizeEntrySearch(`${entry.diceRoll} ${entry.name} ${entry.effect}`),
            }));
            this.updateFilter();
        }));
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }

    updateFilter() {
        if (!this.entries) {
            return;
        }
        const terms = normalizeEntrySearch(this.filter).split(/\s+/).filter(t => t);
        this.visibleEntries = this.entries.filter(e => terms.every(t => e.searchText.includes(t)));
    }

    clearFilter() {
        this.filter = '';
        this.updateFilter();
    }
}
