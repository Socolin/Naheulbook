import {Component, Input, ViewChild} from '@angular/core';

import {ActiveStatsModifier, LapCountDecrement} from '../shared';
import {AddEffectDialogComponent} from '../effect';

import {Character} from './character.model';
import {CharacterService} from './character.service';
import {MatDialog} from '@angular/material';

@Component({
    selector: 'effect-panel',
    templateUrl: './effect-panel.component.html',
    styleUrls: ['./effect-panel.component.scss'],
})
export class EffectPanelComponent {
    @Input() character: Character;

    public selectedModifier: ActiveStatsModifier | undefined;

    constructor(
        private readonly characterService: CharacterService,
        private readonly dialog: MatDialog,
    ) {
    }

    openAddEffectDialog() {
        const dialogRef = this.dialog.open(AddEffectDialogComponent, {minWidth: '100vw', height: '100vh'});
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }

            if (result.durationType === 'lap') {
                result.lapCountDecrement = new LapCountDecrement();
                result.lapCountDecrement.fighterId = this.character.id;
                result.lapCountDecrement.fighterIsMonster = false;
                result.lapCountDecrement.when = 'BEFORE';
            }

            this.characterService.addModifier(this.character.id, result).subscribe(
                this.character.onAddModifier.bind(this.character)
            );
        });
    }

    removeModifier(modifier: ActiveStatsModifier) {
        this.selectedModifier = undefined;
        this.characterService.removeModifier(this.character.id, modifier.id).subscribe(
            this.character.onRemoveModifier.bind(this.character)
        );
    }

    selectModifier(modifier: ActiveStatsModifier) {
        this.selectedModifier = modifier;
    }

    updateReusableModifier(modifier: ActiveStatsModifier) {
        this.characterService.toggleModifier(this.character.id, modifier.id).subscribe(
            this.character.onUpdateModifier.bind(this.character)
        );
    }
}
