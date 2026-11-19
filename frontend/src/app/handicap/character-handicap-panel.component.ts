import {ChangeDetectionStrategy, Component, Input} from '@angular/core';
import {firstValueFrom} from 'rxjs';
import {Character} from '../character/character.model';
import {NhbkMatDialog} from '../material-workaround';
import {HandicapService} from './handicap.service';
import {HandicapResponse} from '../api/responses';
import {CharacterEntryPanelComponent} from '../shared/entry-group-tabs/character-entry-panel.component';
import {AddEntryDialogComponent, AddEntryDialogData, AddEntryDialogResult} from '../shared/entry-group-tabs/add-entry-dialog.component';
import {EntryView} from '../shared/entry-group-tabs/entry-group-tabs.model';

@Component({
    selector: 'character-handicap-panel',
    imports: [CharacterEntryPanelComponent],
    template: `
        <character-entry-panel title="Handicaps" addLabel="Ajouter un handicap"
                               emptyText="Aucun handicap."
                               [entries]="entries"
                               (add)="openAddDialog()" (remove)="remove($event)"></character-entry-panel>
    `,
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class CharacterHandicapPanelComponent {
    @Input({required: true}) character: Character;

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly handicapService: HandicapService,
    ) {
    }

    get entries(): EntryView[] {
        // The D100 roll only matters when picking a handicap, not once the character has it
        return [...this.character.handicaps]
            .sort((a, b) => a.name.localeCompare(b.name))
            .map(handicap => ({...toHandicapEntryView(handicap), badge: undefined}));
    }

    async openAddDialog() {
        const handicaps = await firstValueFrom(this.handicapService.getHandicaps());
        const knownIds = new Set(this.character.handicaps.map(x => x.id));
        const dialogRef = this.dialog.openFullScreen<AddEntryDialogComponent, AddEntryDialogData, AddEntryDialogResult>(
            AddEntryDialogComponent,
            {
                data: {
                    title: 'Ajouter des handicaps',
                    groups: [{
                        id: 'handicaps',
                        name: 'Handicaps',
                        entries: handicaps.filter(x => !knownIds.has(x.id)).map(toHandicapEntryView),
                    }],
                    warnings: {},
                    diceRollSides: 100,
                }
            }
        );
        const result = await firstValueFrom(dialogRef.afterClosed());
        if (!result) {
            return;
        }
        for (const handicapId of result.selectedIds) {
            const response = await firstValueFrom(this.handicapService.addCharacterHandicap(this.character.id, {handicapId}));
            this.character.addHandicap(response);
        }
    }

    remove(entry: EntryView) {
        this.handicapService.removeCharacterHandicap(this.character.id, entry.id)
            .subscribe(() => this.character.removeHandicap(entry.id));
    }
}

function toHandicapEntryView(handicap: HandicapResponse): EntryView {
    return {
        id: handicap.id,
        name: handicap.name,
        badge: String(handicap.diceRoll),
        restriction: handicap.data.condition,
        effectLines: [handicap.data.effect],
    };
}
