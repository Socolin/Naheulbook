import {Observable} from 'rxjs';

import {map} from 'rxjs/operators';
import {Component, DoCheck, EventEmitter, Input, Output} from '@angular/core';

import {AutocompleteValue} from '../shared';

import {EffectService} from './effect.service';
import {Effect} from './effect.model';
import {IDurable} from '../api/shared';

@Component({
    selector: 'active-effect-editor',
    templateUrl: './active-effect-editor.component.html',
    styleUrls: ['./active-effect-editor.component.scss'],
})
export class ActiveEffectEditorComponent implements DoCheck {
    @Input() reusableToggle = true;
    @Output() validate: EventEmitter<{effect: Effect, data: any}> = new EventEmitter<{effect: Effect, data: any}>();
    @Output() changed: EventEmitter<{effect: Effect, data: any}> = new EventEmitter<{effect: Effect, data: any}>();

    public effectFilterName: string;
    public autocompleteEffectListCallback: Function = this.updateEffectListAutocomplete.bind(this);
    public preSelectedEffect: Effect | undefined;
    public newEffectReusable: boolean;
    public newEffectCustomDuration = false;
    public customDuration: IDurable;

    constructor(
        private readonly effectService: EffectService,
    ) {
    }

    updateEffectListAutocomplete(filter: string): Observable<AutocompleteValue[]> {
        this.effectFilterName = filter;
        return this.effectService.searchEffect(filter).pipe(map(
            list => list.map(e => new AutocompleteValue(e, e.subCategory.name + ': ' + e.name))
        ));
    }

    selectEffectInAutocompleteList(effect: Effect) {
        this.preSelectedEffect = effect;
        this.newEffectCustomDuration = false;
        if (effect) {
            this.updateCustomDuration(effect);
        }
    }

    public updateCustomDuration(effect: Effect) {
        switch (effect.durationType) {
            case 'combat':
                this.customDuration = {durationType: effect.durationType, combatCount: effect.combatCount};
                break;
            case 'time':
                this.customDuration = {durationType: effect.durationType, timeDuration: effect.timeDuration};
                break;
            case 'lap':
                this.customDuration = {durationType: effect.durationType, lapCount: effect.lapCount};
                break;
            case 'custom':
                this.customDuration = {durationType: effect.durationType, duration: effect.duration};
                break;
            case 'forever':
                this.customDuration = {durationType: effect.durationType};
                break;
        }
    }

    updateNewEffect(): {effect: Effect, data: any} | undefined {
        if (!this.preSelectedEffect) {
            return undefined;
        }

        let data = {
            reusable: this.newEffectReusable,
        };
        if (this.newEffectCustomDuration) {
            data['durationType'] = this.customDuration.durationType;
            if (this.customDuration.durationType === 'combat') {
                data['combatCount'] = this.customDuration.combatCount;
            }
            else if (this.customDuration.durationType === 'time') {
                data['timeDuration'] = this.customDuration.timeDuration;
            }
            else if (this.customDuration.durationType === 'lap') {
                data['lapCount'] = this.customDuration.lapCount;
            }
            else if (this.customDuration.durationType === 'custom') {
                data['duration'] = this.customDuration.durationType;
            }
            else if (this.customDuration.durationType === 'forever') {
            }
        }

        return {
            effect: this.preSelectedEffect,
            data: data
        };
    }

    addEffect() {
        let newEffect = this.updateNewEffect();
        if (newEffect) {
            this.validate.emit(newEffect);
        }
    }

    unselectEffect() {
        this.preSelectedEffect = undefined;
        this.effectFilterName = '';
    }

    selectEffect(effect: Effect) {
        this.preSelectedEffect = effect;
        if (effect) {
            this.updateCustomDuration(effect);
        }
    }

    ngDoCheck() {
        let newEffect = this.updateNewEffect();
        if (newEffect) {
            this.changed.emit(newEffect);
        }
    }
}
