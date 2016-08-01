import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable, ReplaySubject} from 'rxjs/Rx';

import {Effect, EffectCategory} from './effect.model';
import {JsonService} from '../shared/json-service';

@Injectable()
export class EffectService extends JsonService {
    private effectsByCategory: {[categoryId: number]: ReplaySubject<Effect[]>} = {};
    private effectCategoryList: ReplaySubject<EffectCategory[]>;

    constructor(http: Http) {
        super(http);
    }

    getCategoryList(): Observable<EffectCategory[]> {
        if (!this.effectCategoryList || this.effectCategoryList.isUnsubscribed) {
            this.effectCategoryList = new ReplaySubject<EffectCategory[]>(1);

            this._http.get('/api/effect/categoryList')
                .map(res => res.json())
                .subscribe(
                    categoryList => {
                        this.effectCategoryList.next(categoryList);
                    },
                    error => {
                        this.effectCategoryList.error(error);
                    }
                );
        }
        return this.effectCategoryList;
    }

    getEffects(categoryId: number): Observable<Effect[]> {
        if (!(categoryId in this.effectsByCategory) || this.effectsByCategory[categoryId].isUnsubscribed) {
            this.effectsByCategory[categoryId] = new ReplaySubject<Effect[]>(1);
            this.postJson('/api/effect/list', {
                categoryId: categoryId
            }).map(res => res.json()).subscribe(
                effects => {
                    this.effectsByCategory[categoryId].next(effects);
                },
                error => {
                    this.effectsByCategory[categoryId].error(error);
                }
            );
        }
        return this.effectsByCategory[categoryId];
    }

    searchEffect(filter: string): Observable<Effect[]> {
        return this.postJson('/api/effect/search', {
            filter: filter
        }).map(res => res.json());
    }

    getEffect(effectId: number): Observable<Effect> {
        return this.postJson('/api/effect/detail', {
            effectId: effectId
        }).map(res => res.json());
    }

    createEffect(effect: Object): Observable<Effect> {
        return this.postJson('/api/effect/create', {
            effect: effect
        }).map(res => res.json());
    }

    editEffect(effect: Object): Observable<Effect> {
        return this.postJson('/api/effect/edit', {
            effect: effect
        }).map(res => res.json());
    }
}
