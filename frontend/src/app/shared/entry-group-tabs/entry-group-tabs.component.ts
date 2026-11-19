import {ChangeDetectionStrategy, Component, EventEmitter, Input, OnChanges, Output} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {MatTab, MatTabGroup, MatTabLabel} from '@angular/material/tabs';
import {MatFormField, MatLabel, MatPrefix, MatSuffix} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatIcon} from '@angular/material/icon';
import {MatIconButton} from '@angular/material/button';
import {EntryGroupView, EntryView, normalizeEntrySearch} from './entry-group-tabs.model';

interface GroupTab {
    group: EntryGroupView;
    descriptionParagraphs: string[];
    entries: { entry: EntryView, searchText: string }[];
    visibleEntries: EntryView[];
}

/**
 * Searchable list of reference entries (talents, masteries), one tab per group
 */
@Component({
    selector: 'entry-group-tabs',
    imports: [
        FormsModule,
        MatTabGroup,
        MatTab,
        MatTabLabel,
        MatFormField,
        MatLabel,
        MatPrefix,
        MatSuffix,
        MatInput,
        MatIcon,
        MatIconButton,
    ],
    templateUrl: './entry-group-tabs.component.html',
    styleUrl: './entry-page.scss',
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class EntryGroupTabsComponent implements OnChanges {
    @Input({required: true}) title!: string;
    @Input({required: true}) groups!: EntryGroupView[];
    @Input() selectedGroupId?: string;
    @Output() selectedGroupIdChange = new EventEmitter<string>();

    tabs: GroupTab[] = [];
    selectedIndex = 0;
    filter = '';

    ngOnChanges() {
        this.tabs = this.groups.map(group => {
            const entries = group.entries.map(entry => ({
                entry,
                searchText: normalizeEntrySearch([entry.name, entry.restriction, ...entry.effectLines].filter(x => x).join(' ')),
            }));
            return {
                group,
                descriptionParagraphs: (group.description ?? '').split('\n').map(p => p.trim()).filter(p => p),
                entries,
                visibleEntries: entries.map(e => e.entry),
            };
        });
        const index = this.tabs.findIndex(t => t.group.id === this.selectedGroupId);
        if (index !== -1) {
            this.selectedIndex = index;
        }
        this.updateFilter();
    }

    selectTab(index: number) {
        if (index === this.selectedIndex) {
            return;
        }
        this.selectedIndex = index;
        this.selectedGroupIdChange.emit(this.tabs[index].group.id);
    }

    updateFilter() {
        const terms = normalizeEntrySearch(this.filter).split(/\s+/).filter(t => t);
        for (const tab of this.tabs) {
            tab.visibleEntries = tab.entries.filter(e => terms.every(t => e.searchText.includes(t))).map(e => e.entry);
        }
    }

    clearFilter() {
        this.filter = '';
        this.updateFilter();
    }
}
