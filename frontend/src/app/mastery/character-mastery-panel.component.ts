import {ChangeDetectionStrategy, Component, Input} from '@angular/core';
import {firstValueFrom} from 'rxjs';
import {Character} from '../character/character.model';
import {NhbkMatDialog} from '../material-workaround';
import {MasteryService} from './mastery.service';
import {MasteryGroupResponse, MasteryResponse} from '../api/responses';
import {CharacterEntryPanelComponent} from '../shared/entry-group-tabs/character-entry-panel.component';
import {AddEntryDialogComponent, AddEntryDialogData, AddEntryDialogResult} from '../shared/entry-group-tabs/add-entry-dialog.component';
import {EntryView, toEntryView} from '../shared/entry-group-tabs/entry-group-tabs.model';
import {buildCandidates, getNamesById, getWarnings, LearnableGroup} from '../shared/entry-group-tabs/entry-candidates';

export function toMasteryLearnableGroups(groups: MasteryGroupResponse[]): LearnableGroup<MasteryResponse>[] {
    return groups.map(g => ({id: g.id, name: g.name, description: g.description, entries: g.masteries}));
}

export function getMasteryPrerequisiteId(mastery: MasteryResponse): string | undefined {
    return mastery.data.prerequisiteMasteryId;
}

@Component({
    selector: 'character-mastery-panel',
    imports: [CharacterEntryPanelComponent],
    template: `
        <character-entry-panel title="Maîtrises" addLabel="Ajouter une maîtrise"
                               emptyText="Aucune maîtrise."
                               [entries]="entries" [warnings]="warnings"
                               (add)="openAddDialog()" (remove)="remove($event)"></character-entry-panel>
    `,
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class CharacterMasteryPanelComponent {
    @Input({required: true}) character: Character;

    private namesById = new Map<string, string>();

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly masteryService: MasteryService,
    ) {
        // Names are used to display the prerequisites
        masteryService.getMasteryGroups().subscribe(groups => {
            this.namesById = getNamesById(toMasteryLearnableGroups(groups));
        });
    }

    get entries(): EntryView[] {
        return [...this.character.masteries]
            .sort((a, b) => a.name.localeCompare(b.name))
            .map(x => toEntryView(x, getMasteryPrerequisiteId(x), this.namesById));
    }

    get warnings(): Record<string, string[]> {
        const knownIds = new Set(this.character.masteries.map(x => x.id));
        return Object.fromEntries(this.character.masteries.map(x => [
            x.id,
            getWarnings(x, getMasteryPrerequisiteId(x), knownIds, this.character.level, this.namesById),
        ]));
    }

    async openAddDialog() {
        const groups = await firstValueFrom(this.masteryService.getMasteryGroups());
        const knownIds = new Set(this.character.masteries.map(x => x.id));
        const candidates = buildCandidates(toMasteryLearnableGroups(groups), knownIds, this.character.level, getMasteryPrerequisiteId);
        const dialogRef = this.dialog.openFullScreen<AddEntryDialogComponent, AddEntryDialogData, AddEntryDialogResult>(
            AddEntryDialogComponent,
            {data: {title: 'Ajouter des maîtrises', ...candidates}}
        );
        const result = await firstValueFrom(dialogRef.afterClosed());
        if (!result) {
            return;
        }
        for (const masteryId of result.selectedIds) {
            const response = await firstValueFrom(this.masteryService.addCharacterMastery(this.character.id, {masteryId}));
            this.character.addMastery(response);
        }
    }

    remove(entry: EntryView) {
        this.masteryService.removeCharacterMastery(this.character.id, entry.id)
            .subscribe(() => this.character.removeMastery(entry.id));
    }
}
