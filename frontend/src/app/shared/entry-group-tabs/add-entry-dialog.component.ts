import {ChangeDetectionStrategy, Component, Inject} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogClose, MatDialogRef} from '@angular/material/dialog';
import {MatButton, MatIconButton} from '@angular/material/button';
import {MatToolbar} from '@angular/material/toolbar';
import {MatIcon} from '@angular/material/icon';
import {MatCheckbox} from '@angular/material/checkbox';
import {MatFormField, MatLabel, MatPrefix} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {EntryGroupView, EntryView, normalizeEntrySearch} from './entry-group-tabs.model';
import {getRandomInt} from '../random';

export type AddEntryDialogData = {
    title: string;
    /** Entries the character can get (the ones already known are left out) */
    groups: EntryGroupView[];
    /** Unmet conditions (level, prerequisite) by entry id: shown, but they do not prevent the selection */
    warnings: Record<string, string[]>;
    /** Offer to roll a die and select the entry whose badge is the result (D100 table of handicaps) */
    diceRollSides?: number;
}

export type AddEntryDialogResult = {
    selectedIds: string[];
}

interface CandidateGroup {
    group: EntryGroupView;
    candidates: { entry: EntryView, searchText: string }[];
    visibleCandidates: EntryView[];
}

@Component({
    selector: 'app-add-entry-dialog',
    imports: [
        FormsModule,
        MatToolbar,
        MatButton,
        MatIconButton,
        MatDialogClose,
        MatIcon,
        MatCheckbox,
        MatFormField,
        MatLabel,
        MatPrefix,
        MatInput,
    ],
    templateUrl: './add-entry-dialog.component.html',
    styleUrl: './add-entry-dialog.component.scss',
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class AddEntryDialogComponent {
    groups: CandidateGroup[];
    filter = '';
    selectedIds = new Set<string>();
    lastRoll?: { value: number, entry?: EntryView };

    constructor(
        private readonly dialogRef: MatDialogRef<AddEntryDialogComponent, AddEntryDialogResult>,
        @Inject(MAT_DIALOG_DATA) public readonly data: AddEntryDialogData,
    ) {
        this.groups = data.groups.map(group => {
            const candidates = group.entries.map(entry => ({
                entry,
                searchText: normalizeEntrySearch([entry.badge, entry.name, entry.restriction, ...entry.effectLines].filter(x => x).join(' ')),
            }));
            return {group, candidates, visibleCandidates: candidates.map(c => c.entry)};
        });
    }

    get hasVisibleCandidates(): boolean {
        return this.groups.some(g => g.visibleCandidates.length);
    }

    updateFilter() {
        const terms = normalizeEntrySearch(this.filter).split(/\s+/).filter(t => t);
        for (const group of this.groups) {
            group.visibleCandidates = group.candidates.filter(c => terms.every(t => c.searchText.includes(t))).map(c => c.entry);
        }
    }

    toggleSelection(id: string) {
        if (this.selectedIds.has(id)) {
            this.selectedIds.delete(id);
        } else {
            this.selectedIds.add(id);
        }
    }

    rollDice() {
        const value = getRandomInt(1, this.data.diceRollSides!);
        const entry = this.groups.flatMap(g => g.candidates).find(c => c.entry.badge === String(value))?.entry;
        this.lastRoll = {value, entry};
        if (entry) {
            this.selectedIds.add(entry.id);
        }
    }

    valid() {
        this.dialogRef.close({selectedIds: [...this.selectedIds]});
    }
}
