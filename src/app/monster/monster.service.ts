import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable} from 'rxjs/Rx';

import {JsonService} from '../shared/json-service';
import {NotificationsService} from '../notifications';
import {LoginService} from '../user';

import {ActiveStatsModifier} from '../shared/stat-modifier.model';
import {Monster} from './monster.model';
import {Skill, SkillService} from '../skill';
import {PartialItem} from '../item';

@Injectable()
export class MonsterService extends JsonService {
    constructor(http: Http
        , notification: NotificationsService
        , loginService: LoginService
        , private _skillService: SkillService) {
        super(http, notification, loginService);
    }

    createMonster(groupId: number, monster): Observable<Monster> {
        return Observable.forkJoin(
            this.postJson('/api/monster/createMonster', {
                monster: monster,
                groupId: groupId
            }).map(res => res.json()),
            this._skillService.getSkillsById()
        ).map(([monsterJsonData, skillsById]: [any, {[skillId: number]: Skill}]) => {
            return Monster.fromJson(monsterJsonData, skillsById)
        });
    }

    updateMonster(monsterId: number, fieldName: string, value: any): Observable<{value: any, fieldName: string}> {
        return this.postJson('/api/monster/updateMonster', {
            fieldName: fieldName,
            value: value,
            monsterId: monsterId
        }).map(res => res.json());
    }

    killMonster(monsterId: number): Observable<Monster> {
        return Observable.forkJoin(
            this.postJson('/api/group/killMonster', {
                monsterId: monsterId
            }).map(res => res.json()),
            this._skillService.getSkillsById()
        ).map(([monsterJsonData, skillsById]: [any, {[skillId: number]: Skill}]) => {
            return Monster.fromJson(monsterJsonData, skillsById)
        });
    }

    deleteMonster(monsterId: number): Observable<any> {
        return this.postJson('/api/group/deleteMonster', {
            monsterId: monsterId
        }).map(res => res.json());
    }

    addModifier(monsterId: number, modifier: ActiveStatsModifier): Observable<ActiveStatsModifier> {
        return this.postJson('/api/monster/addModifier', {
            monsterId: monsterId,
            modifier: modifier,
        }).map(res => ActiveStatsModifier.fromJson(res.json()));
    }

    removeModifier(monsterId: number, modifierId: number): Observable<ActiveStatsModifier> {
        return this.postJson('/api/monster/removeModifier', {
            monsterId: monsterId,
            modifierId: modifierId,
        }).map(res => ActiveStatsModifier.fromJson(res.json()));
    }

    toggleModifier(monsterId: number, modifierId: number): Observable<ActiveStatsModifier> {
        return this.postJson('/api/monster/toggleModifier', {
            monsterId: monsterId,
            modifierId: modifierId,
        }).map(res => ActiveStatsModifier.fromJson(res.json()));
    }

    equipItem(monsterId: number, itemId: number, equiped: boolean): Observable<PartialItem> {
        return this.postJson('/api/monster/equipItem', {
            monsterId: monsterId,
            itemId: itemId,
            equiped: equiped,
        }).map(res => PartialItem.fromJson(res.json()));
    }
}
