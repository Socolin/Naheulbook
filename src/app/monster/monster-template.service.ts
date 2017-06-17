import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable, ReplaySubject} from 'rxjs/Rx';

import {MonsterTemplate, MonsterTemplateCategory, MonsterTemplateType, MonsterTrait} from './monster.model';
import {JsonService} from '../shared/json-service';
import {NotificationsService} from '../notifications';
import {LoginService} from '../user';

@Injectable()
export class MonsterTemplateService extends JsonService {
    private monsterTypes: ReplaySubject<MonsterTemplateType[]>;
    private monsterTraits: ReplaySubject<MonsterTrait[]>;
    private monsterTraitsById: ReplaySubject<{[id: number]: MonsterTrait}>;

    constructor(http: Http
        , notification: NotificationsService
        , loginService: LoginService) {
        super(http, notification, loginService);
    }

    getMonsterCategoriesById(): Observable<{[id: number]: MonsterTemplateCategory}> {
        return this.getMonsterTypes()
            .map((types: MonsterTemplateType[]) => {
                let categoriesById = {};
                for (let type of types) {
                    for (let category of type.categories) {
                        categoriesById[category.id] = category;
                    }
                }
                return categoriesById;
            });
    }

    getMonsterTypesById(): Observable<{[id: number]: MonsterTemplateType}> {
        return this.getMonsterTypes()
            .map((types: MonsterTemplateType[]) => {
                let typesById = {};
                types.map(c => {typesById[c.id] = c});
                return types;
            });
    }

    getMonsterTypes(): Observable<MonsterTemplateType[]> {
        if (!this.monsterTypes) {
            this.monsterTypes = new ReplaySubject<MonsterTemplateType[]>(1);

            this.postJson('/api/monsterTemplate/ListTypes')
                .map(res => res.json())
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
        return Observable.forkJoin(
            this.getMonsterCategoriesById(),
            this.postJson('/api/monsterTemplate/listMonster').map(res => res.json())
        ).map(([categoriesById, monsterTemplatesDatas]: [{[id: number]: MonsterTemplateCategory}, MonsterTemplate[]]) => {
            return MonsterTemplate.templatessFromJson(monsterTemplatesDatas, categoriesById);
        });
    }

    getMonsterTraits(): Observable<MonsterTrait[]> {
        if (!this.monsterTraits) {
            this.monsterTraits = new ReplaySubject<MonsterTrait[]>(1);

            this.postJson('/api/monsterTemplate/listTraits')
                .map(res => res.json())
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

    getMonsterTraitsById(): Observable<{[id: number]: MonsterTrait}> {
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
        return this.postJson('/api/monsterTemplate/searchMonster', {
            name: name
        }).map(res => res.json());
    }

    editMonster(monster: MonsterTemplate): Observable<MonsterTemplate> {
        let category = monster.category;
        delete monster.category;
        let observable = this.postJson('/api/monsterTemplate/editMonster', {categoryId: category.id, monster: monster})
            .map(res => res.json());
        monster.category = category;
        return observable;
    }

    createType(name: string): Observable<MonsterTemplateType> {
        return this.postJson('/api/monsterTemplate/createType', {
            name: name
        }).map(res => MonsterTemplateType.fromJson(res.json()));
    }

    createCategory(type: MonsterTemplateType, name: string): Observable<MonsterTemplateCategory> {
        return this.postJson('/api/monsterTemplate/createCategory', {
            typeId: type.id,
            name: name
        }).map(res => MonsterTemplateCategory.fromJson(res.json(), type));
    }
}
