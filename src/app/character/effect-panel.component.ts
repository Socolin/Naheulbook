import {Component, OnInit, Input, Output, EventEmitter, ViewChild} from '@angular/core';
import {Portal, OverlayRef} from '@angular/material';

import {NhbkDialogService} from '../shared/nhbk-dialog.service';
import {EffectCategory} from '../effect/effect.model';

import {Character, CharacterEffect, CharacterModifier} from './character.model';
import {CharacterService} from './character.service';
import {CharacterWebsocketService} from './character-websocket.service';
import {EffectService} from '../effect/effect.service';

@Component({
    selector: 'effect-detail',
    templateUrl: './effect-detail.component.html',
    styleUrls: ['./effect-detail.component.scss'],
})
export class EffectDetailComponent {
    @Input() characterEffect: CharacterEffect;
    @Input() effectCategoriesById: {[categoryId: number]: EffectCategory};
    @Output() onRemove: EventEmitter<CharacterModifier> = new EventEmitter<CharacterModifier>();
}

@Component({
    selector: 'modifier-detail',
    templateUrl: './modifier-detail.component.html',
    styleUrls: ['./modifier-detail.component.scss'],
})
export class ModifierDetailComponent {
    @Input() characterModifier: CharacterModifier;
    @Output() onRemove: EventEmitter<CharacterModifier> = new EventEmitter<CharacterModifier>();
}

@Component({
    selector: 'effect-panel',
    templateUrl: './effect-panel.component.html',
    styleUrls: ['./effect-panel.component.scss'],
})
export class EffectPanelComponent implements OnInit {
    @Input() character: Character;
    public selectedEffect: CharacterEffect;
    public selectedModifier: CharacterModifier;

    // Add effect dialog
    @ViewChild('addEffectDialog')
    public addEffectDialog: Portal<any>;
    public addEffectOverlayRef: OverlayRef;
    public addEffectTypeSelectedTab: number = 0;

    public effectCategoriesById: { [categoryId: number]: EffectCategory };

    public newCharacterModifier: CharacterModifier = new CharacterModifier();

    constructor(private _characterWebsocketService: CharacterWebsocketService
        , private _nhbkDialogService: NhbkDialogService
        , private _effectService: EffectService
        , private _characterService: CharacterService) {
    }

    openAddEffectModal() {
        this.newCharacterModifier = new CharacterModifier();
        this.addEffectOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.addEffectDialog);
    }

    closeAddEffectDialog() {
        this.addEffectOverlayRef.detach();
    }

    selectEffectTypeTab(index: number) {
        this.addEffectTypeSelectedTab = index;
    }

    // Called by callback from character-effect-editor
    addEffect(newEffect: any) {
        let effect = newEffect.effect;
        let data = newEffect.data;

        this.closeAddEffectDialog();

        this._characterService.addEffect(this.character.id, effect.id, data).subscribe(
            this.onAddEffect.bind(this)
        );
    }

    onAddEffect(charEffect: CharacterEffect) {
        for (let i = 0; i < this.character.effects.length; i++) {
            if (this.character.effects[i].id === charEffect.id) {
                return;
            }
        }

        this._characterWebsocketService.notifyChange('Ajout de l\'effet: ' + charEffect.effect.name);
        this.character.effects.push(charEffect);
        this.character.update();
    }

    removeEffect(charEffect: CharacterEffect) {
        this.selectedEffect = null;
        this._characterService.removeEffect(this.character.id, charEffect).subscribe(
            this.onRemoveEffect.bind(this)
        );
    }

    onRemoveEffect(charEffect: CharacterEffect) {
        for (let i = 0; i < this.character.effects.length; i++) {
            let e = this.character.effects[i];
            if (e.id === charEffect.id) {
                this._characterWebsocketService.notifyChange('Suppression de l\'effetde: ' + charEffect.effect.name);
                this.character.effects.splice(i, 1);
                this.character.update();
                return;
            }
        }
    }

    selectEffect(charEffect: CharacterEffect) {
        this.selectedModifier = null;
        this.selectedEffect = charEffect;
        return false;
    }

    updateReusableEffect(charEffect: CharacterEffect) {
        this._characterService.toggleEffect(this.character.id, charEffect).subscribe(
            this.onUpdateEffect.bind(this)
        );
    }

    onUpdateEffect(charEffect: CharacterEffect) {
        for (let i = 0; i < this.character.effects.length; i++) {
            if (this.character.effects[i].id === charEffect.id) {
                if (this.character.effects[i].active === charEffect.active
                    && this.character.effects[i].currentTimeDuration === charEffect.currentTimeDuration
                    && this.character.effects[i].currentCombatCount === charEffect.currentCombatCount
                    && this.character.effects[i].currentLapCount === charEffect.currentLapCount) {
                    return;
                }

                if (!this.character.effects[i].active && charEffect.active) {
                    this._characterWebsocketService.notifyChange('Activation de l\'effet: ' + charEffect.effect.name);
                } else if (this.character.effects[i].active && !charEffect.active) {
                    this._characterWebsocketService.notifyChange('Désactivation de l\'effet: ' + charEffect.effect.name);
                } else {
                    this._characterWebsocketService.notifyChange('Mis à jour de l\'effet: ' + charEffect.effect.name);
                }

                this.character.effects[i].active = charEffect.active;
                this.character.effects[i].currentCombatCount = charEffect.currentCombatCount;
                this.character.effects[i].currentTimeDuration = charEffect.currentTimeDuration;
                this.character.effects[i].currentLapCount = charEffect.currentLapCount;
                break;
            }
        }
        this.character.update();
    }

    addCustomModifier() {
        this._characterService.addModifier(this.character.id, this.newCharacterModifier).subscribe(
            this.onAddModifier.bind(this)
        );
    }

    onAddModifier(modifier: CharacterModifier) {
        for (let i = 0; i < this.character.modifiers.length; i++) {
            if (this.character.modifiers[i].id === modifier.id) {
                return;
            }
        }
        this.character.modifiers.push(modifier);
        this.character.update();
        this._characterWebsocketService.notifyChange('Ajout du modificateur: ' + modifier.name);
    }

    removeModifier(modifier: CharacterModifier) {
        this.selectedModifier = null;
        this._characterService.removeModifier(this.character.id, modifier.id).subscribe(
            this.onRemoveModifier.bind(this)
        );
    }

    onRemoveModifier(modifier: CharacterModifier) {
        for (let i = 0; i < this.character.modifiers.length; i++) {
            let e = this.character.modifiers[i];
            if (e.id === modifier.id) {
                this.character.modifiers.splice(i, 1);
                this.character.update();
                this._characterWebsocketService.notifyChange('Suppression du modificateur: ' + modifier.name);
                return;
            }
        }
    }

    selectModifier(modifier: CharacterModifier) {
        this.selectedEffect = null;
        this.selectedModifier = modifier;
        return false;
    }

    updateReusableModifier(modifier: CharacterModifier) {
        this._characterService.toggleModifier(this.character.id, modifier.id).subscribe(
            this.onUpdateModifier.bind(this)
        );
    }

    onUpdateModifier(modifier: CharacterModifier) {
        for (let i = 0; i < this.character.modifiers.length; i++) {
            if (this.character.modifiers[i].id === modifier.id) {
                if (this.character.modifiers[i].active === modifier.active
                    && this.character.modifiers[i].currentTimeDuration === modifier.currentTimeDuration
                    && this.character.modifiers[i].currentLapCount === modifier.currentLapCount
                    && this.character.modifiers[i].currentCombatCount === modifier.currentCombatCount) {
                    return;
                }
                if (!this.character.modifiers[i].active && modifier.active) {
                    this._characterWebsocketService.notifyChange('Activation de l\'effet: ' + modifier.name);
                } else if (this.character.modifiers[i].active && !modifier.active) {
                    this._characterWebsocketService.notifyChange('Désactivation de l\'effet: ' + modifier.name);
                } else {
                    this._characterWebsocketService.notifyChange('Mis à jour de l\'effet: ' + modifier.name);
                }
                this.character.modifiers[i].active = modifier.active;
                this.character.modifiers[i].currentCombatCount = modifier.currentCombatCount;
                this.character.modifiers[i].currentTimeDuration = modifier.currentTimeDuration;
                this.character.modifiers[i].currentLapCount = modifier.currentLapCount;
                break;
            }
        }
        this.character.update();
    }

    ngOnInit() {
        this._characterWebsocketService.registerPacket('addEffect').subscribe(this.onAddEffect.bind(this));
        this._characterWebsocketService.registerPacket('removeEffect').subscribe(this.onRemoveEffect.bind(this));
        this._characterWebsocketService.registerPacket('updateEffect').subscribe(this.onUpdateEffect.bind(this));
        this._characterWebsocketService.registerPacket('addModifier').subscribe(this.onAddModifier.bind(this));
        this._characterWebsocketService.registerPacket('removeModifier').subscribe(this.onRemoveModifier.bind(this));
        this._characterWebsocketService.registerPacket('updateModifier').subscribe(this.onUpdateModifier.bind(this));

        this._effectService.getCategoryList().subscribe(
            categories => {
                this.effectCategoriesById = {};
                categories.map(c => this.effectCategoriesById[c.id] = c);
            });
    }
}
