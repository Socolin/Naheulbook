import {forkJoin, Observable} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {ActiveStatsModifier} from '../shared';
import {Monster} from './monster.model';
import {SkillService} from '../skill';
import {CreateMonsterRequest, MoveMonsterToFightRequest} from '../api/requests';
import {MonsterResponse} from '../api/responses';
import {IActiveStatsModifier} from '../api/shared';

@Injectable({providedIn: 'root'})
export class MonsterService {
    constructor(
        private readonly httpClient: HttpClient,
        private readonly skillService: SkillService
    ) {
    }

    createMonster(groupId: number, monster: CreateMonsterRequest): Observable<Monster> {
        return forkJoin([
            this.httpClient.post<MonsterResponse>(`/api/v2/groups/${groupId}/monsters`, monster),
            this.skillService.getSkillsById()
        ]).pipe(map(([monsterJsonData, skillsById]) => {
            return Monster.fromResponse(monsterJsonData, skillsById)
        }));
    }

    updateMonsterData(monsterId: number, monsterData): Observable<void> {
        return this.httpClient.put<void>(`/api/v2/monsters/${monsterId}/data`, monsterData);
    }

    updateMonster(monsterId: number, request: { name?: string }): Observable<void> {
        return this.httpClient.patch<void>(`/api/v2/monsters/${monsterId}`, request);
    }

    updateMonsterTarget(monsterId: number, request: { id: number, isMonster: boolean }): Observable<void> {
        return this.httpClient.put<void>(`/api/v2/monsters/${monsterId}/target`, request);
    }

    killMonster(monsterId: number): Observable<void> {
        return this.httpClient.post<void>(`/api/v2/monsters/${monsterId}/kill`, {});
    }

    moveMonsterToFight(monsterId: number, request: MoveMonsterToFightRequest) {
        return this.httpClient.post<void>(`/api/v2/monsters/${monsterId}/moveToFight`, request);
    }

    deleteMonster(monsterId: number): Observable<any> {
        return this.httpClient.delete<void>(`/api/v2/monsters/${monsterId}`);
    }

    addModifier(monsterId: number, modifier: ActiveStatsModifier): Observable<ActiveStatsModifier> {
        return this.httpClient.post<IActiveStatsModifier>(`/api/v2/monsters/${monsterId}/modifiers`, modifier)
            .pipe(map(res => ActiveStatsModifier.fromJson(res)));
    }

    removeModifier(monsterId: number, modifierId: number): Observable<void> {
        return this.httpClient.delete<void>(`/api/v2/monsters/${monsterId}/modifiers/${modifierId}`);
    }
}
