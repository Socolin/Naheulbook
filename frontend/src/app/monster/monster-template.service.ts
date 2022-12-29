import {forkJoin, Observable, ReplaySubject} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {
    MonsterTemplate,
    MonsterTemplateSubCategory,
    MonsterTemplateType,
    MonsterTrait,
    MonsterTraitDictionary
} from './monster.model';

import {MonsterTemplateRequest} from '../api/requests';
import {SkillService} from '../skill';
import {
    MonsterSubCategoryResponse,
    MonsterTemplateResponse,
    MonsterTraitResponse,
    MonsterTypeResponse
} from '../api/responses';
import {toDictionary} from '../utils/utils';

@Injectable()
export class MonsterTemplateService {
    private monsterTypes?: ReplaySubject<MonsterTemplateType[]>;
    private monsterTraits?: ReplaySubject<MonsterTrait[]>;
    private monsterTraitsById?: ReplaySubject<MonsterTraitDictionary>;

    constructor(
        private readonly httpClient: HttpClient,
        private readonly skillService: SkillService
    ) {
    }

    getMonsterCategoriesById(): Observable<{ [id: number]: MonsterTemplateSubCategory }> {
        return this.getMonsterTypes().pipe(
            map((types: MonsterTemplateType[]) => {
                let subCategoriesById = {};
                for (let type of types) {
                    for (let subCategory of type.subCategories) {
                        subCategoriesById[subCategory.id] = subCategory;
                    }
                }
                return subCategoriesById;
            }));
    }

    getMonsterTypes(): Observable<MonsterTemplateType[]> {
        if (!this.monsterTypes) {
            const loadingMonsterTypes = new ReplaySubject<MonsterTemplateType[]>(1);
            this.monsterTypes = loadingMonsterTypes;

            this.httpClient.get<MonsterTypeResponse[]>('/api/v2/monsterTypes')
                .subscribe(
                    monsterTypesJsonData => {
                        let monsterTypes = MonsterTemplateType.typesFromJson(monsterTypesJsonData);
                        loadingMonsterTypes.next(monsterTypes);
                        loadingMonsterTypes.complete();
                    },
                    error => {
                        loadingMonsterTypes.error(error);
                    }
                );
        }
        return this.monsterTypes;
    }

    getMonsterList(): Observable<MonsterTemplate[]> {
        return forkJoin([
            this.getMonsterCategoriesById(),
            this.httpClient.get<MonsterTemplateResponse[]>('/api/v2/monsterTemplates'),
            this.skillService.getSkillsById()
        ]).pipe(map(([categoriesById, monsterTemplatesDatas, skillsById]) => {
            return MonsterTemplate.templatesFromResponse(monsterTemplatesDatas, categoriesById, skillsById);
        }));
    }

    getMonsterTraits(): Observable<MonsterTrait[]> {
        if (!this.monsterTraits) {
            const loadingMonsterTraits = new ReplaySubject<MonsterTrait[]>(1);
            this.monsterTraits = loadingMonsterTraits;

            this.httpClient.get<MonsterTraitResponse[]>('/api/v2/monsterTraits')
                .subscribe(
                    monsterTraits => {
                        loadingMonsterTraits.next(monsterTraits);
                        loadingMonsterTraits.complete();
                    },
                    error => {
                        loadingMonsterTraits.error(error);
                    }
                );
        }
        return this.monsterTraits;
    }

    getMonsterTraitsById(): Observable<MonsterTraitDictionary> {
        if (!this.monsterTraitsById) {
            const loadingMonsterTraitsById = new ReplaySubject<MonsterTraitDictionary>(1);
            this.monsterTraitsById = loadingMonsterTraitsById;

            this.getMonsterTraits().subscribe(
                monsterTraits => {
                    loadingMonsterTraitsById.next(toDictionary(monsterTraits));
                    loadingMonsterTraitsById.complete();
                },
                error => {
                    loadingMonsterTraitsById.error(error);
                }
            );
        }
        return this.monsterTraitsById;
    }

    searchMonster(name: string, monsterTypeId?: number, monsterSubCategoryId?: number): Observable<MonsterTemplate[]> {
        let url = `/api/v2/monsterTemplates/search?filter=${encodeURIComponent(name)}`;
        if (monsterSubCategoryId) {
            url += `&monsterSubCategoryId=${encodeURIComponent(monsterSubCategoryId.toString())}`;
        } else if (monsterTypeId) {
            url += `&monsterTypeId=${encodeURIComponent(monsterTypeId.toString())}`;
        }
        return forkJoin([
            this.getMonsterCategoriesById(),
            this.httpClient.get<MonsterTemplateResponse[]>(url),
            this.skillService.getSkillsById()
        ]).pipe(map(([subCategories, monsters, skillsById]) => MonsterTemplate.templatesFromResponse(monsters, subCategories, skillsById)));
    }

    createMonsterTemplate(request: MonsterTemplateRequest): Observable<MonsterTemplate> {
        return forkJoin([
            this.getMonsterCategoriesById(),
            this.httpClient.post<MonsterTemplateResponse>(`/api/v2/monsterTemplates/`, request),
            this.skillService.getSkillsById()
        ]).pipe(map(([categoriesById, monsterResponse, skillsById]) => {
            return MonsterTemplate.fromResponse(monsterResponse, categoriesById, skillsById);
        }));
    }

    editMonsterTemplate(monterTemplateId: number, request: MonsterTemplateRequest): Observable<MonsterTemplate> {
        return forkJoin([
            this.getMonsterCategoriesById(),
            this.httpClient.put<MonsterTemplateResponse>(`/api/v2/monsterTemplates/${monterTemplateId}`, request),
            this.skillService.getSkillsById()
        ]).pipe(map(([categoriesById, monsterResponse, skillsById]) => {
            return MonsterTemplate.fromResponse(monsterResponse, categoriesById, skillsById);
        }));
    }

    createType(name: string): Observable<MonsterTemplateType> {
        return this.httpClient.post<MonsterTypeResponse>('/api/v2/monsterTypes', {
            name: name
        }).pipe(map(response => MonsterTemplateType.fromResponse(response)));
    }

    createSubCategory(type: MonsterTemplateType, name: string): Observable<MonsterTemplateSubCategory> {
        return this.httpClient.post<MonsterSubCategoryResponse>(`/api/v2/monsterTypes/${type.id}/subCategories`, {
            name: name
        }).pipe(map(response => MonsterTemplateSubCategory.fromResponse(response, type)));
    }

    invalidateMonsterTypes() {
        this.monsterTypes = undefined;
    }
}
