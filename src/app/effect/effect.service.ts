import {forkJoin, Observable, ReplaySubject} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {Effect, EffectSubCategory, EffectSubCategoryDictionary, EffectType, EffectTypeDictionary} from './effect.model';
import {CreateEffectSubCategoryRequest, CreateEffectRequest, EditEffectRequest} from '../api/requests';
import {EffectSubCategoryResponse, EffectResponse, EffectTypeResponse} from '../api/responses';

@Injectable()
export class EffectService {
    private effectsBySubCategory: { [subCategoryId: number]: ReplaySubject<Effect[]> } = {};
    private effectTypes?: ReplaySubject<EffectType[]>;
    private effectsById: { [effectId: number]: ReplaySubject<Effect> } = {};

    constructor(private httpClient: HttpClient) {
    }

    getEffectSubCategoriesById(): Observable<EffectSubCategoryDictionary> {
        return this.getEffectTypes().pipe(
            map((types: EffectType[]) => {
                let categoriesById = {};
                for (let type of types) {
                    for (let subCategory of type.subCategories) {
                        categoriesById[subCategory.id] = subCategory;
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
            const loadingEffectTypes = new ReplaySubject<EffectType[]>(1);
            this.effectTypes = loadingEffectTypes;

            this.httpClient.get<any[]>('/api/v2/effectCategories')
                .subscribe(
                    effectTypesJsonData => {
                        loadingEffectTypes.next(EffectType.fromResponses(effectTypesJsonData));
                        loadingEffectTypes.complete();
                    },
                    error => {
                        loadingEffectTypes.error(error);
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
        this.clearCacheSubCategory(effect.subCategory.id);
    }

    clearCacheSubCategory(subCategoryId: number) {
        if (subCategoryId in this.effectsBySubCategory) {
            this.effectsBySubCategory[subCategoryId].unsubscribe();
            delete this.effectsBySubCategory[subCategoryId];
        }
    }

    getEffects(subCategoryId: number): Observable<Effect[]> {
        if (!(subCategoryId in this.effectsBySubCategory)) {
            this.effectsBySubCategory[subCategoryId] = new ReplaySubject<Effect[]>(1);
            forkJoin([
                this.getEffectSubCategoriesById(),
                this.httpClient.get<EffectResponse[]>(`/api/v2/effectCategories/${subCategoryId}/effects`)
            ]).pipe(
                map(([categoriesById, responses]) => Effect.fromResponses(categoriesById, responses))
            ).subscribe(
                effects => {
                    this.effectsBySubCategory[subCategoryId].next(effects);
                    this.effectsBySubCategory[subCategoryId].complete();
                },
                error => {
                    this.effectsBySubCategory[subCategoryId].error(error);
                }
            );
        }
        return this.effectsBySubCategory[subCategoryId];
    }

    searchEffect(filter: string): Observable<Effect[]> {
        return forkJoin([
            this.getEffectSubCategoriesById(),
            this.httpClient.get<EffectResponse[]>('/api/v2/effects/search?filter=' + encodeURIComponent(filter))
        ]).pipe(map(([categoriesById, effectsJsonData]) => {
            return Effect.fromResponses(categoriesById, effectsJsonData);
        }));
    }

    getEffect(effectId: number): Observable<Effect> {
        if (!(effectId in this.effectsById)) {
            this.effectsById[effectId] = new ReplaySubject<Effect>(1);
            forkJoin([
                this.getEffectSubCategoriesById(),
                this.httpClient.get<EffectResponse>(`/api/v2/effects/${effectId}`)
            ]).pipe(map(([categoriesById, effectJsonData]: [EffectSubCategoryDictionary, EffectResponse]) => {
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

    createEffect(effectSubCategoryId: number, request: CreateEffectRequest): Observable<Effect> {
        return forkJoin([
            this.getEffectSubCategoriesById(),
            this.httpClient.post<EffectResponse>(`/api/v2/effectSubCategories/${effectSubCategoryId}/effects`, request)
        ]).pipe(map(([categoriesById, response]) =>
            Effect.fromResponse(response, categoriesById)
        ));
    }

    editEffect(effectId: number, request: EditEffectRequest): Observable<Effect> {
        return forkJoin([
            this.getEffectSubCategoriesById(),
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

    createSubCategory(type: EffectType, name: string, diceSize: number, diceCount: number, note?: string): Observable<EffectSubCategory> {
        return this.httpClient.post<EffectSubCategoryResponse>('/api/v2/effectSubCategories', {
            typeId: type.id,
            name: name,
            note: note,
            diceCount: diceSize,
            diceSize: diceSize
        } as CreateEffectSubCategoryRequest).pipe(map(res => {
            const subCategory = EffectSubCategory.fromResponse(res, type);
            type.subCategories.push(subCategory);
            return subCategory;
        }));
    }
}
