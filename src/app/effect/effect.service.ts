import {forkJoin, Observable, ReplaySubject} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {
    Effect,
    EffectCategory,
    EffectCategoryDictionary,
    EffectType,
    EffectTypeDictionary
} from './effect.model';
import {CreateEffectCategoryRequest, CreateEffectRequest, EditEffectRequest} from '../api/requests';
import {EffectCategoryResponse, EffectResponse, EffectTypeResponse} from '../api/responses';

@Injectable()
export class EffectService {
    private effectsByCategory: { [categoryId: number]: ReplaySubject<Effect[]> } = {};
    private effectTypes: ReplaySubject<EffectType[]>;
    private effectsById: { [effectId: number]: ReplaySubject<Effect> } = {};

    constructor(private httpClient: HttpClient) {
    }

    getEffectCategoriesById(): Observable<EffectCategoryDictionary> {
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

    getEffectTypesById(): Observable<EffectTypeDictionary> {
        return this.getEffectTypes().pipe(
            map((types: EffectType[]) => {
                let typesById = {};
                types.map(c => {
                    typesById[c.id] = c
                });
                return types;
            }));
    }

    getEffectTypes(): Observable<EffectType[]> {
        if (!this.effectTypes) {
            this.effectTypes = new ReplaySubject<EffectType[]>(1);

            this.httpClient.get<any[]>('/api/v2/effectCategories')
                .subscribe(
                    effectTypesJsonData => {
                        let effectTypes = EffectType.fromResponses(effectTypesJsonData);
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
                this.httpClient.get<EffectResponse[]>(`/api/v2/effectCategories/${categoryId}/effects`)
            ]).pipe(
                map(([categoriesById, responses]) => Effect.fromResponses(categoriesById, responses))
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
        return forkJoin([
            this.getEffectCategoriesById(),
            this.httpClient.get<EffectResponse[]>('/api/v2/effects/search?filter=' + encodeURIComponent(filter))
        ]).pipe(map(([categoriesById, effectsJsonData]) => {
            return Effect.fromResponses(categoriesById, effectsJsonData);
        }));
    }

    getEffect(effectId: number): Observable<Effect> {
        if (!(effectId in this.effectsById)) {
            this.effectsById[effectId] = new ReplaySubject<Effect>(1);
            forkJoin([
                this.getEffectCategoriesById(),
                this.httpClient.get<EffectResponse>(`/api/v2/effects/${effectId}`)
            ]).pipe(map(([categoriesById, effectJsonData]: [EffectCategoryDictionary, EffectResponse]) => {
                return Effect.fromResponse(effectJsonData, categoriesById);
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

    invalidateEffectTypes() {
        this.effectTypes = undefined;
    }

    createEffect(effectCategoryId: number, request: CreateEffectRequest): Observable<Effect> {
        return forkJoin([
            this.getEffectCategoriesById(),
            this.httpClient.post<EffectResponse>(`/api/v2/effectCategories/${effectCategoryId}/effects`, request)
        ]).pipe(map(([categoriesById, response]) =>
            Effect.fromResponse(response, categoriesById)
        ));
    }

    editEffect(effectId: number, request: EditEffectRequest): Observable<Effect> {
        return forkJoin([
            this.getEffectCategoriesById(),
            this.httpClient.put<EffectResponse>(`/api/v2/effects/${effectId}`, request)
        ]).pipe(map(([categoriesById, response]) =>
            Effect.fromResponse(response, categoriesById)
        ));
    }

    createType(name: string): Observable<EffectType> {
        return this.httpClient.post<EffectTypeResponse>('/api/v2/effectTypes', {
            name: name
        }).pipe(map(res => EffectType.fromResponse(res)));
    }

    createCategory(type: EffectType, name: string, diceSize: number, diceCount: number, note?: string): Observable<EffectCategory> {
        return this.httpClient.post<EffectCategoryResponse>('/api/v2/effectCategories', {
            typeId: type.id,
            name: name,
            note: note,
            diceCount: diceSize,
            diceSize: diceSize
        } as CreateEffectCategoryRequest).pipe(map(res => {
            const category = EffectCategory.fromResponse(res, type);
            type.categories.push(category);
            return category;
        }));
    }
}
