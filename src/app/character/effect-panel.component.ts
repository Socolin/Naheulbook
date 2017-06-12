import {Component, Input, Output, EventEmitter, ViewChild} from '@angular/core';

import {ActiveEffect} from '../effect/effect.model';

import {Character} from './character.model';
import {CharacterService} from './character.service';
import {ActiveStatsModifier} from '../shared/stat-modifier.model';
import {AddEffectModalComponent} from '../effect/add-effect-modal.component';

@Component({
    selector: 'effect-detail',
    templateUrl: './effect-detail.component.html',
    styleUrls: ['./effect-detail.component.scss'],
})
export class EffectDetailComponent {
    @Input() characterEffect: ActiveEffect;
    @Output() onRemove: EventEmitter<ActiveEffect> = new EventEmitter<ActiveEffect>();
}

@Component({
    selector: 'effect-panel',
    templateUrl: './effect-panel.component.html',
    styleUrls: ['./effect-panel.component.scss'],
})
export class EffectPanelComponent {
    @Input() character: Character;

    public selectedEffect: ActiveEffect;
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
        this._characterService.addModifier(this.character.id, modifier).subscribe(
            (activeModifier: ActiveStatsModifier) => {
                this.character.onAddModifier(activeModifier);
            }
        );
    }

    removeEffect(charEffect: ActiveEffect) {
        this.selectedEffect = null;
        this._characterService.removeEffect(this.character.id, charEffect).subscribe(
            this.character.onRemoveEffect.bind(this.character)
        );
    }

    selectEffect(charEffect: ActiveEffect) {
        this.selectedModifier = null;
        this.selectedEffect = charEffect;
        return false;
    }

    updateReusableEffect(charEffect: ActiveEffect) {
        this._characterService.toggleEffect(this.character.id, charEffect).subscribe(
            this.character.onUpdateEffect.bind(this.character)
        );
    }

    addCustomModifier(modifier: ActiveStatsModifier) {
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
        this.selectedEffect = null;
        this.selectedModifier = modifier;
    }

    updateReusableModifier(modifier: ActiveStatsModifier) {
        this._characterService.toggleModifier(this.character.id, modifier.id).subscribe(
            this.character.onUpdateModifier.bind(this.character)
        );
    }
}
