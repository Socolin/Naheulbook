import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable, ReplaySubject} from 'rxjs/Rx';

import {MonsterTemplate, MonsterTemplateCategory, MonsterTrait} from './monster.model';
import {JsonService} from '../shared/json-service';
import {NotificationsService} from '../notifications';
import {LoginService} from '../user';

@Injectable()
export class MonsterService extends JsonService {
    private monsterCategories: ReplaySubject<MonsterTemplateCategory[]>;
    private monsterTraits: ReplaySubject<MonsterTrait[]>;
    private monsterTraitsById: ReplaySubject<{[id: number]: MonsterTrait}>;

    constructor(http: Http
        , notification: NotificationsService
        , loginService: LoginService) {
        super(http, notification, loginService);
    }

    getMonsterCategories(): Observable<MonsterTemplateCategory[]> {
        if (!this.monsterCategories) {
            this.monsterCategories = new ReplaySubject<MonsterTemplateCategory[]>(1);

            this.postJson('/api/monster/listCategory')
                .map(res => res.json())
                .subscribe(
                    monsterCategories => {
                        this.monsterCategories.next(monsterCategories);
                        this.monsterCategories.complete();
                    },
                    error => {
                        this.monsterCategories.error(error);
                    }
                );
        }
        return this.monsterCategories;
    }

    getMonsterList(): Observable<MonsterTemplate[]> {
        return this.postJson('/api/monster/listMonster').map(res => res.json());
    }

    getMonsterTraits(): Observable<MonsterTrait[]> {
        if (!this.monsterTraits) {
            this.monsterTraits = new ReplaySubject<MonsterTrait[]>(1);

            this.postJson('/api/monster/listTraits')
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
        return this.postJson('/api/monster/searchMonster', {
            name: name
        }).map(res => res.json());
    }

    editMonster(monster: MonsterTemplate): Observable<MonsterTemplate> {
        return this.postJson('/api/monster/editMonster', monster).map(res => res.json());
    }

}
