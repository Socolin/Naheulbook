import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable, ReplaySubject} from 'rxjs/Rx';

import {Effect, EffectCategory, EffectJsonData} from './effect.model';
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
                .map(res => EffectCategory.categoriesFromJson(res.json()))
                .subscribe(
                    categoryList => {
                        this.effectCategoryList.next(categoryList);
                        this.effectCategoryList.complete();
                    },
                    error => {
                        this.effectCategoryList.error(error);
                    }
                );
        }
        return this.effectCategoryList;
    }

    getCategoriesById(): Observable<{[categoryId: number]: EffectCategory}> {
        return this.getCategoryList().map((categories: EffectCategory[]) => {
            let categoriesById = {};
            categories.map(c => categoriesById[c.id] = c);
            return categoriesById;
        });
    }

    clearCacheEffect(effect: Effect) {
        let effectId = effect.id;
        if (effectId in this.effectsById) {
            this.effectsById[effectId].unsubscribe();
            delete this.effectsById[effectId];
        }
        this.clearCacheCategory(effect.category.id);
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
            Observable.forkJoin(
                this.getCategoriesById(),
                this.postJson('/api/effect/list', {
                    categoryId: categoryId
                }).map(res => res.json())
            ).map(([categoriesById, effectsJsonData]: [{[categoryId: number]: EffectCategory}, EffectJsonData[]]) =>
                Effect.effectsFromJson(categoriesById, effectsJsonData)
            ).subscribe(
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
        return Observable.forkJoin(
            this.getCategoriesById(),
            this.postJson('/api/effect/search', {
                filter: filter
            }).map(res => res.json())
        ).map(([categoriesById, effectsJsonData]: [{[categoryId: number]: EffectCategory}, EffectJsonData[]]) => {
            return Effect.effectsFromJson(categoriesById, effectsJsonData);
        });
    }

    getEffect(effectId: number): Observable<Effect> {
        if (!(effectId in this.effectsById)) {
            this.effectsById[effectId] = new ReplaySubject<Effect>(1);
            Observable.forkJoin(
                this.getCategoriesById(),
                this.postJson('/api/effect/detail', {
                    effectId: effectId
                }).map(res => res.json())
            ).map(([categoriesById, effectJsonData]: [{[categoryId: number]: EffectCategory}, EffectJsonData]) => {
                return Effect.fromJson(effectJsonData, categoriesById);
            }).subscribe(
                (effect: Effect) => {
                    console.log(effect);
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
        return Observable.forkJoin(
            this.getCategoriesById(),
            this.postJson('/api/effect/create', {
                effect: effect
            }).map(res => res.json())
        ).map(([categoriesById, effectJsonData]: [{[categoryId: number]: EffectCategory}, EffectJsonData]) =>
            Effect.fromJson(effectJsonData, categoriesById)
        );
    }

    editEffect(effect: Object): Observable<Effect> {
        return Observable.forkJoin(
            this.getCategoriesById(),
            this.postJson('/api/effect/edit', {
                effect: effect
            }).map(res => res.json())
        ).map(([categoriesById, effectJsonData]: [{[categoryId: number]: EffectCategory}, EffectJsonData]) =>
            Effect.fromJson(effectJsonData, categoriesById)
        );
    }
}
