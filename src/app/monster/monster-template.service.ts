import {forkJoin, Observable, ReplaySubject} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {
    MonsterTemplate,
    MonsterTemplateCategory,
    MonsterTemplateType,
    MonsterTrait,
    MonsterTraitDictionary
} from './monster.model';

import {MonsterTemplateRequest} from '../api/requests';
import {SkillService} from '../skill';
import {MonsterTemplateResponse} from '../api/responses/monster-template-response';
import {MonsterCategoryResponse} from '../api/responses/monster-category-response';
import {MonsterTypeResponse} from '../api/responses';

@Injectable()
export class MonsterTemplateService {
    private monsterTypes?: ReplaySubject<MonsterTemplateType[]>;
    private monsterTraits?: ReplaySubject<MonsterTrait[]>;
    private monsterTraitsById?: ReplaySubject<MonsterTraitDictionary>;

    constructor(
        private httpClient: HttpClient,
        private skillService: SkillService
    ) {
    }

    getMonsterCategoriesById(): Observable<{ [id: number]: MonsterTemplateCategory }> {
        return this.getMonsterTypes().pipe(
            map((types: MonsterTemplateType[]) => {
                let categoriesById = {};
                for (let type of types) {
                    for (let category of type.categories) {
                        categoriesById[category.id] = category;
                    }
                }
                return categoriesById;
            }));
    }

    getMonsterTypes(): Observable<MonsterTemplateType[]> {
        if (!this.monsterTypes) {
            this.monsterTypes = new ReplaySubject<MonsterTemplateType[]>(1);

            this.httpClient.get<MonsterTypeResponse[]>('/api/v2/monsterTypes')
                .subscribe(
                    monsterTypesJsonData => {
                        let monsterTypes = MonsterTemplateType.typesFromJson(monsterTypesJsonData);
                        this.monsterTypes.next(monsterTypes);
                        this.monsterTypes.complete();
                    },
                    error => {
                        this.monsterTypes.error(error);
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
            this.monsterTraits = new ReplaySubject<MonsterTrait[]>(1);

            this.httpClient.get<MonsterTrait[]>('/api/v2/monsterTraits')
                .subscribe(
                    monsterTraits => {
                        this.monsterTraits.next(monsterTraits);
                        this.monsterTraits.complete();
                    },
                    error => {
                        this.monsterTraits.error(error);
                    }
                );
        }
        return this.monsterTraits;
    }

    getMonsterTraitsById(): Observable<MonsterTraitDictionary> {
        if (!this.monsterTraitsById) {
            this.monsterTraitsById = new ReplaySubject<MonsterTrait[]>(1);

            this.getMonsterTraits().subscribe(
                traits => {
                    let monsterTraitsById = {};
                    for (let i = 0; i < traits.length; i++) {
                        let trait = traits[i];
                        monsterTraitsById[trait.id] = trait;
                    }
                    this.monsterTraitsById.next(monsterTraitsById);
                    this.monsterTraitsById.complete();
                },
                error => {
                    this.monsterTraitsById.error(error);
                }
            );
        }
        return this.monsterTraitsById;
    }

    searchMonster(name): Observable<MonsterTemplate[]> {
        return forkJoin([
            this.getMonsterCategoriesById(),
            this.httpClient.get<MonsterTemplateResponse[]>(`/api/v2/monsterTemplates/search?filter=${encodeURIComponent(name)}`),
            this.skillService.getSkillsById()
        ]).pipe(map(([categories, monsters, skillsById]) => MonsterTemplate.templatesFromResponse(monsters, categories, skillsById)));
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

    createCategory(type: MonsterTemplateType, name: string): Observable<MonsterTemplateCategory> {
        return this.httpClient.post<MonsterCategoryResponse>(`/api/v2/monsterTypes/${type.id}/categories`, {
            name: name
        }).pipe(map(response => MonsterTemplateCategory.fromResponse(response, type)));
    }

    invalidateMonsterTypes() {
        this.monsterTypes = undefined;
    }
}
