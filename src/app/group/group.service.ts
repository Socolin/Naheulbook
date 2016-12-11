import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable} from 'rxjs/Rx';

import {HistoryEntry} from '../shared';
import {Monster} from '../monster';
import {JsonService} from '../shared/json-service';
import {NotificationsService} from '../notifications/notifications.service';
import {LoginService} from '../user/login.service';
import {CharacterGiveDestination} from '../character/character.model';
import {GroupData} from './group.model';
import {NhbkDateOffset} from '../date/date.model';
import {Loot} from '../loot';

@Injectable()
export class GroupService extends JsonService {
    constructor(http: Http
        , notification: NotificationsService
        , loginService: LoginService) {
        super(http, notification, loginService);
    }

    /* Monster */

    loadMonsters(groupId: number): Observable<Monster[]> {
        return this.postJson('/api/group/loadMonsters', {
            groupId: groupId
        }).map(res => res.json());
    }

    loadDeadMonsters(groupId: number, startIndex: number, count: number): Observable<Monster[]> {
        return this.postJson('/api/group/loadDeadMonsters', {
            groupId: groupId,
            startIndex: startIndex,
            count: count
        }).map(res => res.json());
    }

    createMonster(groupId: number, monster): Observable<Monster> {
        return this.postJson('/api/group/createMonster', {
            monster: monster,
            groupId: groupId
        }).map(res => res.json());
    }

    updateMonster(monsterId: number, fieldName: string, value: any): Observable<{value: any, fieldName: string}> {
        return this.postJson('/api/group/updateMonster', {
            fieldName: fieldName,
            value: value,
            monsterId: monsterId
        }).map(res => res.json());
    }

    killMonster(monsterId: number): Observable<Monster> {
        return this.postJson('/api/group/killMonster', {
            monsterId: monsterId
        }).map(res => res.json());
    }

    deleteMonster(monsterId: number): Observable<any> {
        return this.postJson('/api/group/deleteMonster', {
            monsterId: monsterId
        }).map(res => res.json());
    }

    /* Loot */

    loadLoots(groupId: number): Observable<Loot[]> {
        return this.postJson('/api/group/loadLoots', {
            groupId: groupId
        }).map(res => res.json());
    }

    createLoot(groupId: number, lootName: string): Observable<Loot> {
        return this.postJson('/api/group/createLoot', {
            groupId: groupId,
            name: lootName
        }).map(res => res.json());
    }

    deleteLoot(lootId: number): Observable<Loot> {
        return this.postJson('/api/group/deleteLoot', {
            lootId: lootId
        }).map(res => res.json());
    }

    updateLoot(loot: Loot): Observable<Loot> {
        return this.postJson('/api/group/updateLoot', {
            loot: loot
        }).map(res => res.json());
    }

    /* History */

    loadHistory(groupId: number, page: number): Observable<HistoryEntry[]> {
        return this.postJson('/api/group/history', {
            groupId: groupId,
            page: page
        }).map(res => res.json());
    }

    addLog(groupId: number, info: string, is_gm: boolean): Observable<Object> {
        return this.postJson('/api/group/addLog', {
            groupId: groupId,
            is_gm: is_gm,
            info: info
        }).map(res => res.json());
    }

    /* Misc */

    editGroupValue(groupId: number, key: string, value: any): Observable<GroupData> {
        return this.postJson('/api/group/edit', {
            groupId: groupId,
            key: key,
            value: value
        }).map(res => res.json());
    }

    addTime(groupId: number, dateOffset: NhbkDateOffset): Observable<GroupData> {
        return this.postJson('/api/group/addTime', {
            groupId: groupId,
            dateOffset: dateOffset
        }).map(res => res.json());
    }

    nextLap(groupId: number): Observable<GroupData> {
        return this.postJson('/api/group/nextLap', {
            groupId: groupId
        }).map(res => res.json());
    }

    listActiveCharactersInGroup(characterId: number): Observable<CharacterGiveDestination[]> {
        return this.postJson('/api/group/listActiveCharacters', {
            characterId: characterId
        }).map(res => res.json());
    }
}
