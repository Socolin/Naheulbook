import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable, ReplaySubject} from 'rxjs/Rx';

import {Effect, EffectCategory} from './effect.model';
import {JsonService} from '../shared/json-service';
import {NotificationsService} from '../notifications';
import {LoginService} from '../user';

@Injectable()
export class EffectService extends JsonService {
    private effectsByCategory: {[categoryId: number]: ReplaySubject<Effect[]>} = {};
    private effectCategoryList: ReplaySubject<EffectCategory[]>;
    private effectsById: {[effectId: number]: ReplaySubject<Effect>} = {};

    constructor(http: Http
        , notification: NotificationsService
        , loginService: LoginService) {
        super(http, notification, loginService);
    }

    getCategoryList(): Observable<EffectCategory[]> {
        if (!this.effectCategoryList) {
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

    clearCacheEffect(effect: Effect) {
        let effectId = effect.id;
        if (effectId in this.effectsById) {
            this.effectsById[effectId].unsubscribe();
            delete this.effectsById[effectId];
        }
        this.clearCacheCategory(effect.category);
    }

    clearCacheCategory(categoryId: number) {
        if (categoryId in this.effectsByCategory) {
            this.effectsByCategory[categoryId].unsubscribe();
            delete this.effectsByCategory[categoryId];
        }
    }

    getEffects(categoryId: number): Observable<Effect[]> {
        if (!(categoryId in this.effectsByCategory)) {
            this.effectsByCategory[categoryId] = new ReplaySubject<Effect[]>(1);
            this.postJson('/api/effect/list', {
                categoryId: categoryId
            }).map(res => res.json()).subscribe(
                effects => {
                    this.effectsByCategory[categoryId].next(effects);
                    this.effectsByCategory[categoryId].complete();
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
        if (!(effectId in this.effectsById)) {
            this.effectsById[effectId] = new ReplaySubject<Effect>(1);
            this.postJson('/api/effect/detail', {
                effectId: effectId
            }).map(res => res.json()).subscribe(
                effect => {
                    this.effectsById[effectId].next(effect);
                    this.effectsById[effectId].complete();
                },
                error => {
                    this.effectsById[effectId].error(error);
                }
            );
        }
        return this.effectsById[effectId];
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
