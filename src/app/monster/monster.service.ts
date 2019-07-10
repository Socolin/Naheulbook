import {forkJoin, Observable} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {ActiveStatsModifier} from '../shared';
import {Monster} from './monster.model';
import {Skill, SkillService} from '../skill';
import {PartialItem} from '../item';

@Injectable()
export class MonsterService {
    constructor(private httpClient: HttpClient
        , private _skillService: SkillService) {
    }

    createMonster(groupId: number, monster): Observable<Monster> {
        return forkJoin([
            this.httpClient.post<Monster>(`/api/v2/groups/${groupId}/monsters`, monster),
            this._skillService.getSkillsById()
        ]).pipe(map(([monsterJsonData, skillsById]: [any, {[skillId: number]: Skill}]) => {
            return Monster.fromJson(monsterJsonData, skillsById)
        }));
    }

    updateMonster(monsterId: number, fieldName: string, value: any): Observable<{value: any, fieldName: string}> {
        return this.httpClient.post<{value: any, fieldName: string}>('/api/monster/updateMonster', {
            fieldName: fieldName,
            value: value,
            monsterId: monsterId
        });
    }

    killMonster(monsterId: number): Observable<Monster> {
        return forkJoin([
            this.httpClient.post('/api/group/killMonster', {
                monsterId: monsterId
            }),
            this._skillService.getSkillsById()
        ]).pipe(map(([monsterJsonData, skillsById]: [any, {[skillId: number]: Skill}]) => {
            return Monster.fromJson(monsterJsonData, skillsById)
        }));
    }

    deleteMonster(monsterId: number): Observable<any> {
        return this.httpClient.post('/api/group/deleteMonster', {
            monsterId: monsterId
        });
    }

    addModifier(monsterId: number, modifier: ActiveStatsModifier): Observable<ActiveStatsModifier> {
        return this.httpClient.post(`/api/v2/monsters/${monsterId}/modifiers`, modifier)
            .pipe(map(res => ActiveStatsModifier.fromJson(res)));
    }

    removeModifier(monsterId: number, modifierId: number): Observable<void> {
        return this.httpClient.delete<void>(`/api/v2/monsters/${monsterId}/modifiers/${modifierId}`);
    }

    toggleModifier(monsterId: number, modifierId: number): Observable<ActiveStatsModifier> {
        return this.httpClient.post('/api/monster/toggleModifier', {
            monsterId: monsterId,
            modifierId: modifierId,
        }).pipe(map(res => ActiveStatsModifier.fromJson(res)));
    }

    equipItem(monsterId: number, itemId: number, equiped: boolean): Observable<PartialItem> {
        return this.httpClient.post('/api/monster/equipItem', {
            monsterId: monsterId,
            itemId: itemId,
            equiped: equiped,
        }).pipe(map(res => PartialItem.fromJson(res)));
    }
}
