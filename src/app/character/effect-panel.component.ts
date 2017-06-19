import {Component, Input, ViewChild} from '@angular/core';

import {Character} from './character.model';
import {CharacterService} from './character.service';
import {ActiveStatsModifier, LapCountDecrement} from '../shared/stat-modifier.model';
import {AddEffectModalComponent} from '../effect/add-effect-modal.component';

@Component({
    selector: 'effect-panel',
    templateUrl: './effect-panel.component.html',
    styleUrls: ['./effect-panel.component.scss'],
})
export class EffectPanelComponent {
    @Input() character: Character;

    public selectedModifier: ActiveStatsModifier;

    // Add effect dialog
    @ViewChild('addEffectModal')
    public addEffectModal: AddEffectModalComponent;

    constructor(private _characterService: CharacterService) {
    }

    // Called by callback from active-effect-editor
    addEffect(newEffect: any) {
        let effect = newEffect.effect;
        let data = newEffect.data;

        this.addEffectModal.close();

        let modifier = ActiveStatsModifier.fromEffect(effect, data);
        if (modifier.durationType === 'lap') {
            modifier.lapCountDecrement = new LapCountDecrement();
            modifier.lapCountDecrement.fighterId = this.character.id;
            modifier.lapCountDecrement.fighterIsMonster = false;
            modifier.lapCountDecrement.when = 'BEFORE';
        }

        this._characterService.addModifier(this.character.id, modifier).subscribe(
            (activeModifier: ActiveStatsModifier) => {
                this.character.onAddModifier(activeModifier);
            }
        );
    }

    addCustomModifier(modifier: ActiveStatsModifier) {
        if (modifier.durationType === 'lap') {
            modifier.lapCountDecrement = new LapCountDecrement();
            modifier.lapCountDecrement.fighterId = this.character.id;
            modifier.lapCountDecrement.fighterIsMonster = false;
            modifier.lapCountDecrement.when = 'BEFORE';
        }
        this._characterService.addModifier(this.character.id, modifier).subscribe(
            this.character.onAddModifier.bind(this.character)
        );
    }

    removeModifier(modifier: ActiveStatsModifier) {
        this.selectedModifier = null;
        this._characterService.removeModifier(this.character.id, modifier.id).subscribe(
            this.character.onRemoveModifier.bind(this.character)
        );
    }

    selectModifier(modifier: ActiveStatsModifier) {
        this.selectedModifier = modifier;
    }

    updateReusableModifier(modifier: ActiveStatsModifier) {
        this._characterService.toggleModifier(this.character.id, modifier.id).subscribe(
            this.character.onUpdateModifier.bind(this.character)
        );
    }
}
