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

    updateMonsterData(monsterId: number, monsterData): Observable<void> {
        return this.httpClient.put<void>(`/api/v2/monsters/${monsterId}/data`, monsterData);
    }

    updateMonster(monsterId: number, request: {name?: string}): Observable<void> {
        return this.httpClient.patch<void>(`/api/v2/monsters/${monsterId}`, request);
    }

    updateMonsterTarget(monsterId: number, request: {id: number, isMonster: boolean}): Observable<void> {
        return this.httpClient.put<void>(`/api/v2/monsters/${monsterId}/target`, request);
    }

    killMonster(monsterId: number): Observable<void> {
        return this.httpClient.post<void>(`/api/v2/monsters/${monsterId}/kill`, {});
    }

    deleteMonster(monsterId: number): Observable<any> {
        return this.httpClient.delete<void>(`/api/v2/monsters/${monsterId}`);
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
