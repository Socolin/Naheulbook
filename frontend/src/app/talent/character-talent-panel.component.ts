import {ChangeDetectionStrategy, Component, Input} from '@angular/core';
import {firstValueFrom} from 'rxjs';
import {Character} from '../character/character.model';
import {NhbkMatDialog} from '../material-workaround';
import {TalentService} from './talent.service';
import {TalentGroupResponse, TalentResponse} from '../api/responses';
import {CharacterEntryPanelComponent} from '../shared/entry-group-tabs/character-entry-panel.component';
import {AddEntryDialogComponent, AddEntryDialogData, AddEntryDialogResult} from '../shared/entry-group-tabs/add-entry-dialog.component';
import {EntryGroupView, EntryView, toEntryView} from '../shared/entry-group-tabs/entry-group-tabs.model';

@Component({
    selector: 'character-talent-panel',
    imports: [CharacterEntryPanelComponent],
    template: `
        <character-entry-panel title="Talents" addLabel="Ajouter un talent"
                               emptyText="Aucun talent."
                               [entries]="entries" [warnings]="warnings"
                               (add)="openAddDialog()" (remove)="remove($event)"></character-entry-panel>
    `,
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class CharacterTalentPanelComponent {
    @Input({required: true}) character: Character;

    private namesById = new Map<string, string>();

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly talentService: TalentService,
    ) {
        // Names are used to display the prerequisites
        talentService.getTalentGroups().subscribe(groups => {
            this.namesById = new Map(groups.flatMap(g => g.talents).map(x => [x.id, x.name]));
        });
    }

    get entries(): EntryView[] {
        return [...this.character.talents]
            .sort((a, b) => a.name.localeCompare(b.name))
            .map(x => toEntryView(x, x.data.prerequisiteTalentId, this.namesById));
    }

    get warnings(): Record<string, string[]> {
        const knownIds = new Set(this.character.talents.map(x => x.id));
        return Object.fromEntries(this.character.talents.map(x => [x.id, this.getWarnings(x, knownIds)]));
    }

    async openAddDialog() {
        const groups = await firstValueFrom(this.talentService.getTalentGroups());
        const knownIds = new Set(this.character.talents.map(x => x.id));
        const all = groups.flatMap(g => g.talents);
        const dialogRef = this.dialog.openFullScreen<AddEntryDialogComponent, AddEntryDialogData, AddEntryDialogResult>(
            AddEntryDialogComponent,
            {
                data: {
                    title: 'Ajouter des talents',
                    groups: groups.map(group => this.toCandidateGroup(group, knownIds)),
                    warnings: Object.fromEntries(all.map(x => [x.id, this.getWarnings(x, knownIds)])),
                }
            }
        );
        const result = await firstValueFrom(dialogRef.afterClosed());
        if (!result) {
            return;
        }
        for (const talentId of result.selectedIds) {
            const response = await firstValueFrom(this.talentService.addCharacterTalent(this.character.id, {talentId}));
            this.character.addTalent(response);
        }
    }

    remove(entry: EntryView) {
        this.talentService.removeCharacterTalent(this.character.id, entry.id)
            .subscribe(() => this.character.removeTalent(entry.id));
    }

    private toCandidateGroup(group: TalentGroupResponse, knownIds: Set<string>): EntryGroupView {
        return {
            id: group.id,
            name: group.name,
            description: group.description,
            entries: group.talents
                .filter(x => !knownIds.has(x.id))
                .map(x => toEntryView(x, x.data.prerequisiteTalentId, this.namesById)),
        };
    }

    private getWarnings(talent: TalentResponse, knownIds: Set<string>): string[] {
        const warnings: string[] = [];
        if (talent.data.requiredLevel && talent.data.requiredLevel > this.character.level) {
            warnings.push(`Niveau ${talent.data.requiredLevel} requis`);
        }
        const prerequisiteId = talent.data.prerequisiteTalentId;
        if (prerequisiteId && !knownIds.has(prerequisiteId)) {
            warnings.push(`Requiert « ${this.namesById.get(prerequisiteId) ?? '?'} »`);
        }
        return warnings;
    }
}
