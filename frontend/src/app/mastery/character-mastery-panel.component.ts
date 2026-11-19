import {ChangeDetectionStrategy, Component, Input} from '@angular/core';
import {firstValueFrom} from 'rxjs';
import {Character} from '../character/character.model';
import {NhbkMatDialog} from '../material-workaround';
import {MasteryService} from './mastery.service';
import {MasteryGroupResponse, MasteryResponse} from '../api/responses';
import {CharacterEntryPanelComponent} from '../shared/entry-group-tabs/character-entry-panel.component';
import {AddEntryDialogComponent, AddEntryDialogData, AddEntryDialogResult} from '../shared/entry-group-tabs/add-entry-dialog.component';
import {EntryGroupView, EntryView, toEntryView} from '../shared/entry-group-tabs/entry-group-tabs.model';

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
            this.namesById = new Map(groups.flatMap(g => g.masteries).map(x => [x.id, x.name]));
        });
    }

    get entries(): EntryView[] {
        return [...this.character.masteries]
            .sort((a, b) => a.name.localeCompare(b.name))
            .map(x => toEntryView(x, x.data.prerequisiteMasteryId, this.namesById));
    }

    get warnings(): Record<string, string[]> {
        const knownIds = new Set(this.character.masteries.map(x => x.id));
        return Object.fromEntries(this.character.masteries.map(x => [x.id, this.getWarnings(x, knownIds)]));
    }

    async openAddDialog() {
        const groups = await firstValueFrom(this.masteryService.getMasteryGroups());
        const knownIds = new Set(this.character.masteries.map(x => x.id));
        const all = groups.flatMap(g => g.masteries);
        const dialogRef = this.dialog.openFullScreen<AddEntryDialogComponent, AddEntryDialogData, AddEntryDialogResult>(
            AddEntryDialogComponent,
            {
                data: {
                    title: 'Ajouter des maîtrises',
                    groups: groups.map(group => this.toCandidateGroup(group, knownIds)),
                    warnings: Object.fromEntries(all.map(x => [x.id, this.getWarnings(x, knownIds)])),
                }
            }
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

    private toCandidateGroup(group: MasteryGroupResponse, knownIds: Set<string>): EntryGroupView {
        return {
            id: group.id,
            name: group.name,
            description: group.description,
            entries: group.masteries
                .filter(x => !knownIds.has(x.id))
                .map(x => toEntryView(x, x.data.prerequisiteMasteryId, this.namesById)),
        };
    }

    private getWarnings(mastery: MasteryResponse, knownIds: Set<string>): string[] {
        const warnings: string[] = [];
        if (mastery.data.requiredLevel && mastery.data.requiredLevel > this.character.level) {
            warnings.push(`Niveau ${mastery.data.requiredLevel} requis`);
        }
        const prerequisiteId = mastery.data.prerequisiteMasteryId;
        if (prerequisiteId && !knownIds.has(prerequisiteId)) {
            warnings.push(`Requiert « ${this.namesById.get(prerequisiteId) ?? '?'} »`);
        }
        return warnings;
    }
}
