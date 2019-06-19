import {forkJoin, Observable, ReplaySubject} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {Effect, EffectCategory, EffectJsonData, EffectType} from './effect.model';

@Injectable()
export class EffectService {
    private effectsByCategory: {[categoryId: number]: ReplaySubject<Effect[]>} = {};
    private effectTypes: ReplaySubject<EffectType[]>;
    private effectsById: {[effectId: number]: ReplaySubject<Effect>} = {};

    constructor(private httpClient: HttpClient) {
    }

    getEffectCategoriesById(): Observable<{[id: number]: EffectCategory}> {
        return this.getEffectTypes().pipe(
            map((types: EffectType[]) => {
                let categoriesById = {};
                for (let type of types) {
                    for (let category of type.categories) {
                        categoriesById[category.id] = category;
                    }
                }
                return categoriesById;
            }));
    }

    getEffectTypesById(): Observable<{[id: number]: EffectType}> {
        return this.getEffectTypes().pipe(
            map((types: EffectType[]) => {
                let typesById = {};
                types.map(c => {typesById[c.id] = c});
                return types;
            }));
    }

    getEffectTypes(): Observable<EffectType[]> {
        if (!this.effectTypes) {
            this.effectTypes = new ReplaySubject<EffectType[]>(1);

            this.httpClient.get<any[]>('/api/v2/effectCategories')
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
            forkJoin([
                this.getEffectCategoriesById(),
                this.httpClient.get(`/api/v2/effectCategories/${categoryId}/effects`)
            ]).pipe(map(([categoriesById, effectsJsonData]: [{[categoryId: number]: EffectCategory}, EffectJsonData[]]) =>
                Effect.effectsFromJson(categoriesById, effectsJsonData)
            )).subscribe(
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
        return forkJoin([
            this.getEffectCategoriesById(),
            this.httpClient.post<EffectJsonData[]>('/api/effect/search', {
                filter: filter
            })
        ]).pipe(map(([categoriesById, effectsJsonData]: [{[categoryId: number]: EffectCategory}, EffectJsonData[]]) => {
            return Effect.effectsFromJson(categoriesById, effectsJsonData);
        }));
    }

    getEffect(effectId: number): Observable<Effect> {
        if (!(effectId in this.effectsById)) {
            this.effectsById[effectId] = new ReplaySubject<Effect>(1);
            forkJoin([
                this.getEffectCategoriesById(),
                this.httpClient.post('/api/effect/detail', {
                    effectId: effectId
                })
            ]).pipe(map(([categoriesById, effectJsonData]: [{[categoryId: number]: EffectCategory}, EffectJsonData]) => {
                return Effect.fromJson(effectJsonData, categoriesById);
            })).subscribe(
                (effect: Effect) => {
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
        return forkJoin([
            this.getEffectCategoriesById(),
            this.httpClient.post('/api/effect/create', {
                effect: effect.toJsonData()
            })
        ]).pipe(map(([categoriesById, effectJsonData]: [{[categoryId: number]: EffectCategory}, EffectJsonData]) =>
            Effect.fromJson(effectJsonData, categoriesById)
        ));
    }

    editEffect(effect: Effect): Observable<Effect> {
        return forkJoin([
            this.getEffectCategoriesById(),
            this.httpClient.post('/api/effect/edit', {
                effect: effect.toJsonData()
            })
        ]).pipe(map(([categoriesById, effectJsonData]: [{[categoryId: number]: EffectCategory}, EffectJsonData]) =>
            Effect.fromJson(effectJsonData, categoriesById)
        ));
    }

    createType(name: string): Observable<EffectType> {
        return this.httpClient.post('/api/effect/createType', {
            name: name
        }).pipe(map(res => EffectType.fromJson(res)));
    }

    createCategory(type: EffectType, name: string): Observable<EffectCategory> {
        return this.httpClient.post('/api/effect/createCategory', {
            typeId: type.id,
            name: name
        }).pipe(map(res => EffectCategory.fromJson(res, type)));
    }
}
