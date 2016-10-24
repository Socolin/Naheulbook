import {Component, OnInit, Input, Output, EventEmitter} from "@angular/core";
import {Character, CharacterEffect, CharacterModifier} from "./character.model";
import {Effect, EffectCategory} from "../effect/effect.model";
import {EffectService} from "../effect/effect.service";
import {CharacterService} from "./character.service";
import {Observable} from "rxjs";
import {CharacterWebsocketService} from "./character-websocket.service";
import {AutocompleteValue} from "../shared/autocomplete-input.component";


@Component({
    selector: 'effect-detail',
    templateUrl: 'effect-detail.component.html',
})
export class EffectDetailComponent {
    @Input() characterEffect: CharacterEffect;
    @Input() effectCategoriesById: {[categoryId: number]: EffectCategory};
    @Output() onRemove: EventEmitter<CharacterModifier> = new EventEmitter<CharacterModifier>();
}

@Component({
    selector: 'modifier-detail',
    templateUrl: 'modifier-detail.component.html',
})
export class ModifierDetailComponent {
    @Input() characterModifier: CharacterModifier;
    @Output() onRemove: EventEmitter<CharacterModifier> = new EventEmitter<CharacterModifier>();
}

@Component({
    selector: 'effect-panel',
    templateUrl: 'effect-panel.component.html',
    styles: [`
        .effect-detail-swipe-container
        {
            width: 90vw;
            padding: 10px;
            border-bottom-left-radius: 7px;
            border-top-left-radius: 7px;
        }
        .effect-detail {
            position: absolute;
            top: 10px;
            right: -17px;
            color: #555;
            white-space: nowrap;
            z-index: 100;
            width: 20vw;
            overflow: hidden;
            touch-action: none;
            -webkit-transition: width 1s; /* Safari */
            transition: width 1s;
        }
        .effect-detail-swipe {
            width: 90vw;
            -webkit-transition: width 1s; /* Safari */
            transition: width 1s;
        }
        `
    ],
})
export class EffectPanelComponent implements OnInit {
    @Input() character: Character;
    private selectedEffect: CharacterEffect;
    private selectedModifier: CharacterModifier;
    private showEffectDetail: boolean;

    private preSelectedEffect: Effect;
    private newEffectReusable: boolean;

    private effectFilterName: string;
    private effectCategoriesById: {[categoryId: number]: EffectCategory};
    private autocompleteEffectListCallback: Function = this.updateEffectListAutocomplete.bind(this);


    constructor(private _effectService: EffectService
        , private _characterWebsocketService: CharacterWebsocketService
        , private _characterService: CharacterService) {
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
    }


    addEffect(effect: Effect, reusable: boolean) {
        this._characterService.addEffect(this.character.id, effect.id, reusable).subscribe(
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
        if (this.selectedEffect === charEffect) {
            this.selectedEffect = null;
            return false;
        }
        this.selectedModifier = null;
        this.selectedEffect = charEffect;
        return false;
    }

    toggleReusableEffect(charEffect: CharacterEffect) {
        this._characterService.toggleEffect(this.character.id, charEffect).subscribe(
            this.onUpdateEffect.bind(this)
        );
    }

    onUpdateEffect(charEffect: CharacterEffect) {
        for (let i = 0; i < this.character.effects.length; i++) {
            if (this.character.effects[i].id === charEffect.id) {
                if (this.character.effects[i].active === charEffect.active
                    && this.character.effects[i].currentCombatCount === charEffect.currentCombatCount) {
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
                break;
            }
        }
        this.character.update();
    }


// Custom modifier

    private newModifierCombatCount: boolean;
    public customAddModifier: CharacterModifier = new CharacterModifier();

    addCustomModifier() {
        if (this.customAddModifier.name) {
            if (!this.newModifierCombatCount) {
                this.customAddModifier.combatCount = null;
            } else {
                this.customAddModifier.duration = null;
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
        if (this.selectedModifier === modifier) {
            this.selectedModifier = null;
            return false;
        }
        this.selectedEffect = null;
        this.selectedModifier = modifier;
        return false;
    }

    toggleReusableModifier(modifier: CharacterModifier) {
        this._characterService.toggleModifier(this.character.id, modifier.id).subscribe(
            this.onUpdateModifier.bind(this)
        );
    }

    onUpdateModifier(modifier: CharacterModifier) {
        for (let i = 0; i < this.character.modifiers.length; i++) {
            if (this.character.modifiers[i].id === modifier.id) {
                if (this.character.modifiers[i].active === modifier.active
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

        this._characterWebsocketService.registerPacket("addEffect").subscribe(this.onAddEffect.bind(this));
        this._characterWebsocketService.registerPacket("removeEffect").subscribe(this.onRemoveEffect.bind(this));
        this._characterWebsocketService.registerPacket("updateEffect").subscribe(this.onUpdateEffect.bind(this));
        this._characterWebsocketService.registerPacket("addModifier").subscribe(this.onAddModifier.bind(this));
        this._characterWebsocketService.registerPacket("removeModifier").subscribe(this.onRemoveModifier.bind(this));
        this._characterWebsocketService.registerPacket("updateModifier").subscribe(this.onUpdateModifier.bind(this));
    }
}
