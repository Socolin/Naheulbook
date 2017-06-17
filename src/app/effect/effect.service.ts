import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable, ReplaySubject} from 'rxjs/Rx';

import {Effect, EffectCategory, EffectJsonData, EffectType} from './effect.model';
import {JsonService} from '../shared/json-service';
import {NotificationsService} from '../notifications';
import {LoginService} from '../user';

@Injectable()
export class EffectService extends JsonService {
    private effectsByCategory: {[categoryId: number]: ReplaySubject<Effect[]>} = {};
    private effectTypes: ReplaySubject<EffectType[]>;
    private effectCategoryList: ReplaySubject<EffectCategory[]>;
    private effectsById: {[effectId: number]: ReplaySubject<Effect>} = {};

    constructor(http: Http
        , notification: NotificationsService
        , loginService: LoginService) {
        super(http, notification, loginService);
    }

    getEffectCategoriesById(): Observable<{[id: number]: EffectCategory}> {
        return this.getEffectTypes()
            .map((types: EffectType[]) => {
                let categoriesById = {};
                for (let type of types) {
                    for (let category of type.categories) {
                        categoriesById[category.id] = category;
                    }
                }
                return categoriesById;
            });
    }

    getEffectTypesById(): Observable<{[id: number]: EffectType}> {
        return this.getEffectTypes()
            .map((types: EffectType[]) => {
                let typesById = {};
                types.map(c => {typesById[c.id] = c});
                return types;
            });
    }

    getEffectTypes(): Observable<EffectType[]> {
        if (!this.effectTypes) {
            this.effectTypes = new ReplaySubject<EffectType[]>(1);

            this.postJson('/api/effect/ListTypes')
                .map(res => res.json())
                .subscribe(
                    effectTypesJsonData => {
                        let effectTypes = EffectType.typesFromJson(effectTypesJsonData);
                        this.effectTypes.next(effectTypes);
                        this.effectTypes.complete();
                    },
                    error => {
                        this.effectTypes.error(error);
                    }
                );
        }
        return this.effectTypes;
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
                this.getEffectCategoriesById(),
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
            this.getEffectCategoriesById(),
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
                this.getEffectCategoriesById(),
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

    createEffect(effect: Effect): Observable<Effect> {
        return Observable.forkJoin(
            this.getEffectCategoriesById(),
            this.postJson('/api/effect/create', {
                effect: effect.toJsonData()
            }).map(res => res.json())
        ).map(([categoriesById, effectJsonData]: [{[categoryId: number]: EffectCategory}, EffectJsonData]) =>
            Effect.fromJson(effectJsonData, categoriesById)
        );
    }

    editEffect(effect: Effect): Observable<Effect> {
        return Observable.forkJoin(
            this.getEffectCategoriesById(),
            this.postJson('/api/effect/edit', {
                effect: effect.toJsonData()
            }).map(res => res.json())
        ).map(([categoriesById, effectJsonData]: [{[categoryId: number]: EffectCategory}, EffectJsonData]) =>
            Effect.fromJson(effectJsonData, categoriesById)
        );
    }

    createType(name: string): Observable<EffectType> {
        return this.postJson('/api/effect/createType', {
            name: name
        }).map(res => EffectType.fromJson(res.json()));
    }

    createCategory(type: EffectType, name: string): Observable<EffectCategory> {
        return this.postJson('/api/effect/createCategory', {
            typeId: type.id,
            name: name
        }).map(res => EffectCategory.fromJson(res.json(), type));
    }
}
