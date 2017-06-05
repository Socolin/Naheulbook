import {Component, OnInit, Input, Output, EventEmitter, ViewChild} from '@angular/core';
import {Portal, OverlayRef} from '@angular/material';

import {NhbkDialogService} from '../shared/nhbk-dialog.service';
import {ActiveEffect, EffectCategory} from '../effect/effect.model';

import {Character} from './character.model';
import {CharacterService} from './character.service';
import {EffectService} from '../effect/effect.service';
import {ActiveStatsModifier} from '../shared/stat-modifier.model';

@Component({
    selector: 'effect-detail',
    templateUrl: './effect-detail.component.html',
    styleUrls: ['./effect-detail.component.scss'],
})
export class EffectDetailComponent {
    @Input() characterEffect: ActiveEffect;
    @Input() effectCategoriesById: {[categoryId: number]: EffectCategory};
    @Output() onRemove: EventEmitter<ActiveEffect> = new EventEmitter<ActiveEffect>();
}

@Component({
    selector: 'effect-panel',
    templateUrl: './effect-panel.component.html',
    styleUrls: ['./effect-panel.component.scss'],
})
export class EffectPanelComponent implements OnInit {
    @Input() character: Character;
    public selectedEffect: ActiveEffect;
    public selectedModifier: ActiveStatsModifier;

    // Add effect dialog
    @ViewChild('addEffectDialog')
    public addEffectDialog: Portal<any>;
    public addEffectOverlayRef: OverlayRef;
    public addEffectTypeSelectedTab = 0;

    public effectCategoriesById: { [categoryId: number]: EffectCategory };

    public newActiveStatsModifier: ActiveStatsModifier = new ActiveStatsModifier();

    constructor(private _nhbkDialogService: NhbkDialogService
        , private _effectService: EffectService
        , private _characterService: CharacterService) {
    }

    openAddEffectModal() {
        this.newActiveStatsModifier = new ActiveStatsModifier();
        this.addEffectOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.addEffectDialog);
    }

    closeAddEffectDialog() {
        this.addEffectOverlayRef.detach();
    }

    selectEffectTypeTab(index: number) {
        this.addEffectTypeSelectedTab = index;
    }

    // Called by callback from active-effect-editor
    addEffect(newEffect: any) {
        let effect = newEffect.effect;
        let data = newEffect.data;

        this.closeAddEffectDialog();

        this._characterService.addEffect(this.character.id, effect.id, data).subscribe(
            (activeEffect: ActiveEffect) => {
                this.character.onAddEffect(activeEffect);
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

    addCustomModifier() {
        this._characterService.addModifier(this.character.id, this.newActiveStatsModifier).subscribe(
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
        return false;
    }

    updateReusableModifier(modifier: ActiveStatsModifier) {
        this._characterService.toggleModifier(this.character.id, modifier.id).subscribe(
            this.character.onUpdateModifier.bind(this.character)
        );
    }

    ngOnInit() {
        this._effectService.getCategoryList().subscribe(
            categories => {
                this.effectCategoriesById = {};
                categories.map(c => this.effectCategoriesById[c.id] = c);
            });
    }
}
