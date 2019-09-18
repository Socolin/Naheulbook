import {forkJoin, Observable, ReplaySubject} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {MonsterTemplate, MonsterTemplateCategory, MonsterTemplateType, MonsterTrait} from './monster.model';

import {CreateMonsterTemplateRequest} from '../api/requests';
import {SkillService} from '../skill';
import {MonsterTemplateResponse} from '../api/responses/monster-template-response';

@Injectable()
export class MonsterTemplateService {
    private monsterTypes: ReplaySubject<MonsterTemplateType[]>;
    private monsterTraits: ReplaySubject<MonsterTrait[]>;
    private monsterTraitsById: ReplaySubject<{ [id: number]: MonsterTrait }>;

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

    getMonsterTypesById(): Observable<{ [id: number]: MonsterTemplateType }> {
        return this.getMonsterTypes().pipe(
            map((types: MonsterTemplateType[]) => {
                let typesById = {};
                types.map(c => {
                    typesById[c.id] = c
                });
                return types;
            }));
    }

    getMonsterTypes(): Observable<MonsterTemplateType[]> {
        if (!this.monsterTypes) {
            this.monsterTypes = new ReplaySubject<MonsterTemplateType[]>(1);

            this.httpClient.get<MonsterTemplateType[]>('/api/v2/monsterTypes')
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

    getMonsterTraitsById(): Observable<{ [id: number]: MonsterTrait }> {
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

    addMonster(request: CreateMonsterTemplateRequest): Observable<MonsterTemplate> {
        return forkJoin([
            this.getMonsterCategoriesById(),
            this.httpClient.post<MonsterTemplateResponse>(`/api/v2/monsterTemplates/`, request),
            this.skillService.getSkillsById()
        ]).pipe(map(([categoriesById, monsterResponse, skillsById]) => {
            return MonsterTemplate.fromResponse(monsterResponse, categoriesById, skillsById);
        }));
    }

    editMonster(monster: MonsterTemplate): Observable<MonsterTemplate> {
        let category = monster.category;
        delete monster.category;
        let observable = this.httpClient.put<MonsterTemplate>(`/api/v2/monsterTemplates/${monster.id}`, {...monster});
        monster.category = category;
        return observable;
    }

    createType(name: string): Observable<MonsterTemplateType> {
        return this.httpClient.post<MonsterTemplateType>('/api/v2/monsterTypes', {
            name: name
        }).pipe(map(res => MonsterTemplateType.fromJson(res)));
    }

    createCategory(type: MonsterTemplateType, name: string): Observable<MonsterTemplateCategory> {
        return this.httpClient.post<MonsterTemplateCategory>(`/api/v2/monsterTypes/${type.id}/categories`, {
            name: name
        }).pipe(map(res => MonsterTemplateCategory.fromJson(res, type)));
    }
}
