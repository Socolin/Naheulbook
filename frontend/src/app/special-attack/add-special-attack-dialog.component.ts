import {ChangeDetectionStrategy, Component, Inject, OnInit} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogClose, MatDialogRef} from '@angular/material/dialog';
import {MatButton, MatIconButton} from '@angular/material/button';
import {MatToolbar} from '@angular/material/toolbar';
import {MatIcon} from '@angular/material/icon';
import {MatCheckbox} from '@angular/material/checkbox';
import {MatFormField, MatLabel, MatPrefix} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {SpecialAttackService} from './special-attack.service';
import {SpecialAttackResponse} from '../api/responses';
import {normalizeForSearch, specialAttackSearchText} from './special-attack-view';
import {SpecialAttackCardComponent} from './special-attack-card.component';

export type AddSpecialAttackDialogData = {
    characterLevel: number;
    /** Known special attacks, with their success count (used to check the prerequisites) */
    knownSpecialAttacks: { specialAttackId: string, successCount: number }[];
}

export type AddSpecialAttackDialogResult = {
    selectedSpecialAttackIds: string[];
}

interface Candidate {
    specialAttack: SpecialAttackResponse;
    searchText: string;
    warnings: string[];
}

interface CandidateGroup {
    name: string;
    candidates: Candidate[];
    visibleCandidates: Candidate[];
}

@Component({
    selector: 'app-add-special-attack-dialog',
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
        MatProgressSpinner,
        SpecialAttackCardComponent,
    ],
    templateUrl: './add-special-attack-dialog.component.html',
    styleUrl: './add-special-attack-dialog.component.scss',
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class AddSpecialAttackDialogComponent implements OnInit {
    groups?: CandidateGroup[];
    filter = '';
    selectedIds = new Set<string>();
    expandedId?: string;

    constructor(
        private readonly dialogRef: MatDialogRef<AddSpecialAttackDialogComponent, AddSpecialAttackDialogResult>,
        private readonly specialAttackService: SpecialAttackService,
        @Inject(MAT_DIALOG_DATA) public readonly data: AddSpecialAttackDialogData,
    ) {
    }

    ngOnInit() {
        const known = new Map(this.data.knownSpecialAttacks.map(k => [k.specialAttackId, k.successCount]));
        this.specialAttackService.getSpecialAttackGroups().subscribe(groups => {
            const allSpecialAttacks = new Map(groups.flatMap(g => g.specialAttacks).map(a => [a.id, a]));
            this.groups = groups.map(group => {
                const candidates = group.specialAttacks
                    .filter(specialAttack => !known.has(specialAttack.id))
                    .map(specialAttack => ({
                        specialAttack,
                        searchText: specialAttackSearchText(specialAttack),
                        warnings: this.getWarnings(specialAttack, known, allSpecialAttacks),
                    }));
                return {name: group.name, candidates, visibleCandidates: candidates};
            });
        });
    }

    updateFilter() {
        const terms = normalizeForSearch(this.filter).split(/\s+/).filter(t => t);
        for (const group of this.groups ?? []) {
            group.visibleCandidates = group.candidates.filter(c => terms.every(t => c.searchText.includes(t)));
        }
    }

    toggleSelection(specialAttackId: string) {
        if (this.selectedIds.has(specialAttackId)) {
            this.selectedIds.delete(specialAttackId);
        } else {
            this.selectedIds.add(specialAttackId);
        }
    }

    toggleDetails(specialAttackId: string) {
        this.expandedId = this.expandedId === specialAttackId ? undefined : specialAttackId;
    }

    valid() {
        this.dialogRef.close({selectedSpecialAttackIds: [...this.selectedIds]});
    }

    private getWarnings(
        specialAttack: SpecialAttackResponse,
        known: Map<string, number>,
        allSpecialAttacks: Map<string, SpecialAttackResponse>
    ): string[] {
        const warnings: string[] = [];
        if (specialAttack.data.requiredLevel && specialAttack.data.requiredLevel > this.data.characterLevel) {
            warnings.push(`Niveau ${specialAttack.data.requiredLevel} requis`);
        }
        const prerequisite = specialAttack.data.prerequisite;
        if (prerequisite && (known.get(prerequisite.specialAttackId) ?? 0) < prerequisite.successCount) {
            const name = allSpecialAttacks.get(prerequisite.specialAttackId)?.name ?? '?';
            warnings.push(`Nécessite ${prerequisite.successCount} réussites de « ${name} »`);
        }
        return warnings;
    }
}
