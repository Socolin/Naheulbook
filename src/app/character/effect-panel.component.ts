import {Component, OnInit, Input, Output, EventEmitter, ViewChild} from '@angular/core';
import {Character, CharacterEffect, CharacterModifier} from './character.model';
import {Effect, EffectCategory} from '../effect/effect.model';
import {EffectService} from '../effect/effect.service';
import {CharacterService} from './character.service';
import {Observable} from 'rxjs';
import {CharacterWebsocketService} from './character-websocket.service';
import {AutocompleteValue} from '../shared/autocomplete-input.component';
import {NhbkDateOffset} from '../date/date.model';
import {dateOffset2TimeDuration} from '../date/util';
import {Portal, OverlayRef, Overlay, OverlayState} from '@angular/material';

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
    public showEffectDetail: boolean;

    // Add effect dialog
    @ViewChild('addEffectDialog')
    public addEffectDialog: Portal<any>;
    public addEffectOverlayRef: OverlayRef;

    public preSelectedEffect: Effect;
    public newEffectReusable: boolean;
    public newEffectCustomDuration: boolean = false;
    public newEffectTimeDuration: number = null;
    public newEffectDuration: string = null;
    public newEffectTimeDurationDate: NhbkDateOffset = new NhbkDateOffset();
    public newEffectCombatCount: number = null;

    public effectFilterName: string;
    public newEffectDurationType: string = 'custom';
    public effectCategoriesById: {[categoryId: number]: EffectCategory};
    public autocompleteEffectListCallback: Function = this.updateEffectListAutocomplete.bind(this);

    // Custom modifier
    public newModifierDurationType: string = 'custom';
    public customAddModifier: CharacterModifier = new CharacterModifier();
    public customAddModifierDateOffset: NhbkDateOffset = new NhbkDateOffset();

    constructor(private _effectService: EffectService
        , private _characterWebsocketService: CharacterWebsocketService
        , private _characterService: CharacterService
        , private _overlay: Overlay) {
    }

    openAddEffectModal() {
        let config = new OverlayState();

        config.positionStrategy = this._overlay.position()
            .global()
            .centerHorizontally()
            .centerVertically();
        config.hasBackdrop = true;

        let overlayRef = this._overlay.create(config);
        overlayRef.attach(this.addEffectDialog);
        overlayRef.backdropClick().subscribe(() => overlayRef.detach());
        this.addEffectOverlayRef = overlayRef;

    }

    closeAddEffectDialog(){
        this.addEffectOverlayRef.detach();
    }

    updateEffectListAutocomplete(filter: string): Observable<AutocompleteValue[]> {
        this.effectFilterName = filter;
        if (filter === '') {
            return Observable.from([]);
        }
        return this._effectService.searchEffect(filter).map(
            list => list.map(e => new AutocompleteValue(e, this.effectCategoriesById[e.category].name + ': ' + e.name))
        );
    }

    selectEffectInAutocompleteList(effect: Effect) {
        this.preSelectedEffect = effect;
        this.newEffectCustomDuration = false;
        if (effect) {
            if (effect.combatCount) {
                this.newEffectDurationType = 'combat';
                this.newEffectTimeDuration = null;
                this.newEffectDuration = null;
                this.newEffectCombatCount = effect.combatCount;
            }
            else if (effect.timeDuration) {
                this.newEffectDurationType = 'time';
                this.newEffectTimeDuration = effect.timeDuration;
                this.newEffectDuration = null;
                this.newEffectCombatCount = null;
            }
            else {
                this.newEffectDurationType = 'custom';
                this.newEffectTimeDuration = null;
                this.newEffectDuration = '';
                this.newEffectCombatCount = null;
            }
        }
    }


    setNewEffectTimeDuration(dateOffset: NhbkDateOffset) {
        this.newEffectTimeDuration = dateOffset2TimeDuration(dateOffset);
        this.newEffectTimeDurationDate = dateOffset;
    }

    addEffect(effect: Effect, reusable: boolean) {
        let data = {
            reusable: reusable
        };
        if (this.newEffectCustomDuration) {
            if (this.newEffectDurationType === 'combat') {
                data['combatCount'] = this.newEffectCombatCount;
            }
            else if (this.newEffectDurationType === 'time') {
                data['timeDuration'] = this.newEffectTimeDuration;
            }
            else if (this.newEffectDurationType === 'custom') {
                data['duration'] = this.newEffectDuration;
            }
        }
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

    onSwipeLeftEffect() {
        if (window.innerWidth < 768) {
            this.showEffectDetail = true;
        }
    }

    onSwipeRightEffect() {
        if (window.innerWidth < 768) {
            this.showEffectDetail = false;
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


    // Custom modifier

    setCustomAddModifierTimeDuration(dateOffset: NhbkDateOffset) {
        this.customAddModifier.timeDuration = dateOffset2TimeDuration(dateOffset);
    }

    addCustomModifier() {
        if (this.customAddModifier.name) {
            if (this.newModifierDurationType === 'custom') {
                this.customAddModifier.lapCount = null;
                this.customAddModifier.timeDuration = null;
                this.customAddModifier.combatCount = null;
            } else if (this.newModifierDurationType === 'combat') {
                this.customAddModifier.lapCount = null;
                this.customAddModifier.timeDuration = null;
                this.customAddModifier.duration = null;
            } else if (this.newModifierDurationType === 'time') {
                this.customAddModifier.lapCount = null;
                this.customAddModifier.combatCount = null;
                this.customAddModifier.duration = null;
            } else if (this.newModifierDurationType === 'forever') {
                this.customAddModifier.lapCount = null;
                this.customAddModifier.timeDuration = null;
                this.customAddModifier.combatCount = null;
                this.customAddModifier.duration = null;
            } else if (this.newModifierDurationType === 'lap') {
                this.customAddModifier.timeDuration = null;
                this.customAddModifier.combatCount = null;
                this.customAddModifier.duration = null;
            } else {
                throw new Error('Invalid newModifierDurationType: ' + this.newModifierDurationType);
            }
            this._characterService.addModifier(this.character.id, this.customAddModifier).subscribe(
                this.onAddModifier.bind(this)
            );
        }
    }

    onAddModifier(modifier: CharacterModifier) {
        for (let i = 0; i < this.character.modifiers.length; i++) {
            if (this.character.modifiers[i].id === modifier.id) {
                return;
            }
        }
        this.character.modifiers.push(modifier);
        this.character.update();
        this.customAddModifier = new CharacterModifier();
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
        this._effectService.getCategoryList().subscribe(
            categories => {
                this.effectCategoriesById = {};
                categories.map(c => this.effectCategoriesById[c.id] = c);
            });

        this._characterWebsocketService.registerPacket('addEffect').subscribe(this.onAddEffect.bind(this));
        this._characterWebsocketService.registerPacket('removeEffect').subscribe(this.onRemoveEffect.bind(this));
        this._characterWebsocketService.registerPacket('updateEffect').subscribe(this.onUpdateEffect.bind(this));
        this._characterWebsocketService.registerPacket('addModifier').subscribe(this.onAddModifier.bind(this));
        this._characterWebsocketService.registerPacket('removeModifier').subscribe(this.onRemoveModifier.bind(this));
        this._characterWebsocketService.registerPacket('updateModifier').subscribe(this.onUpdateModifier.bind(this));
    }
}
