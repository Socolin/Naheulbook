import {ChangeDetectionStrategy, Component, Input} from '@angular/core';
import {firstValueFrom} from 'rxjs';
import {Character} from '../character/character.model';
import {NhbkMatDialog} from '../material-workaround';
import {TalentService} from './talent.service';
import {TalentGroupResponse, TalentResponse} from '../api/responses';
import {CharacterEntryPanelComponent} from '../shared/entry-group-tabs/character-entry-panel.component';
import {AddEntryDialogComponent, AddEntryDialogData, AddEntryDialogResult} from '../shared/entry-group-tabs/add-entry-dialog.component';
import {EntryView, toEntryView} from '../shared/entry-group-tabs/entry-group-tabs.model';
import {buildCandidates, getNamesById, getWarnings, LearnableGroup} from '../shared/entry-group-tabs/entry-candidates';

export function toTalentLearnableGroups(groups: TalentGroupResponse[]): LearnableGroup<TalentResponse>[] {
    return groups.map(g => ({id: g.id, name: g.name, description: g.description, entries: g.talents}));
}

export function getTalentPrerequisiteId(talent: TalentResponse): string | undefined {
    return talent.data.prerequisiteTalentId;
}

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
            this.namesById = getNamesById(toTalentLearnableGroups(groups));
        });
    }

    get entries(): EntryView[] {
        return [...this.character.talents]
            .sort((a, b) => a.name.localeCompare(b.name))
            .map(x => toEntryView(x, getTalentPrerequisiteId(x), this.namesById));
    }

    get warnings(): Record<string, string[]> {
        const knownIds = new Set(this.character.talents.map(x => x.id));
        return Object.fromEntries(this.character.talents.map(x => [
            x.id,
            getWarnings(x, getTalentPrerequisiteId(x), knownIds, this.character.level, this.namesById),
        ]));
    }

    async openAddDialog() {
        const groups = await firstValueFrom(this.talentService.getTalentGroups());
        const knownIds = new Set(this.character.talents.map(x => x.id));
        const candidates = buildCandidates(toTalentLearnableGroups(groups), knownIds, this.character.level, getTalentPrerequisiteId);
        const dialogRef = this.dialog.openFullScreen<AddEntryDialogComponent, AddEntryDialogData, AddEntryDialogResult>(
            AddEntryDialogComponent,
            {data: {title: 'Ajouter des talents', ...candidates}}
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
}
