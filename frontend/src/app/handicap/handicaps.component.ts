import {ChangeDetectionStrategy, Component, OnDestroy, OnInit} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {Subscription} from 'rxjs';
import {MatFormField, MatLabel, MatPrefix, MatSuffix} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatIcon} from '@angular/material/icon';
import {MatIconButton} from '@angular/material/button';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {HandicapService} from './handicap.service';
import {HandicapResponse} from '../api/responses';
import {normalizeEntrySearch} from '../shared/entry-group-tabs/entry-group-tabs.model';

interface HandicapItem {
    handicap: HandicapResponse;
    searchText: string;
}

@Component({
    selector: 'app-handicaps',
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
    templateUrl: './handicaps.component.html',
    styleUrls: ['../shared/entry-group-tabs/entry-page.scss', './handicaps.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class HandicapsComponent implements OnInit, OnDestroy {
    handicaps?: HandicapItem[];
    visibleHandicaps: HandicapItem[] = [];
    filter = '';

    private readonly subscription = new Subscription();

    constructor(
        private readonly handicapService: HandicapService,
    ) {
    }

    ngOnInit() {
        this.subscription.add(this.handicapService.getHandicaps().subscribe(handicaps => {
            this.handicaps = handicaps.map(handicap => ({
                handicap,
                searchText: normalizeEntrySearch([handicap.name, handicap.data.condition, handicap.data.effect].filter(x => x).join(' ')),
            }));
            this.updateFilter();
        }));
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }

    updateFilter() {
        if (!this.handicaps) {
            return;
        }
        const terms = normalizeEntrySearch(this.filter).split(/\s+/).filter(t => t);
        this.visibleHandicaps = this.handicaps.filter(h => terms.every(t => h.searchText.includes(t)));
    }

    clearFilter() {
        this.filter = '';
        this.updateFilter();
    }
}
