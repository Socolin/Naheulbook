import {Component, OnInit, Output, EventEmitter, Input, DoCheck} from '@angular/core';
import {Observable} from 'rxjs';

import {EffectCategory, Effect} from './effect.model';
import {IDurable} from '../date/durable.model';
import {AutocompleteValue} from '../shared/autocomplete-input.component';
import {EffectService} from './effect.service';

@Component({
    selector: 'character-effect-editor',
    templateUrl: './character-effect-editor.component.html',
    styleUrls: ['./character-effect-editor.component.scss'],
})
export class CharacterEffectEditorComponent implements OnInit, DoCheck {
    @Input() reusableToggle: boolean = true;
    @Output() onValidate: EventEmitter<any> = new EventEmitter<any>();
    @Output() onChange: EventEmitter<any> = new EventEmitter<any>();

    public effectFilterName: string;
    public effectCategoriesById: { [categoryId: number]: EffectCategory };
    public autocompleteEffectListCallback: Function = this.updateEffectListAutocomplete.bind(this);
    public preSelectedEffect: Effect;
    public newEffectReusable: boolean;
    public newEffectCustomDuration: boolean = false;
    public customDuration: IDurable;

    constructor(private _effectService: EffectService) {
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
    }

    updateNewEffect() {
        let data = {
            reusable: this.newEffectReusable,
        };
        if (this.newEffectCustomDuration) {
            // FIXME: angular4 typescript 2.1 concatenate data and customDuration
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
        if (!this.preSelectedEffect) {
            return null;
        }

        this.onValidate.emit(this.updateNewEffect());
    }

    ngDoCheck() {
        let newEffect = this.updateNewEffect();
        if (newEffect) {
            this.onChange.emit(newEffect);
        }
    }

    ngOnInit(): void {
        this._effectService.getCategoryList().subscribe(
            categories => {
                this.effectCategoriesById = {};
                categories.map(c => this.effectCategoriesById[c.id] = c);
            });
    }
}
