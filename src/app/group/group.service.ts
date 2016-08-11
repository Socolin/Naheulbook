import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable} from 'rxjs/Rx';

import {HistoryEntry} from "../shared";
import {Monster} from "../monster";
import {JsonService} from '../shared/json-service';
import {NotificationsService} from '../notifications/notifications.service';

@Injectable()
export class GroupService extends JsonService {
    constructor(http: Http, notification: NotificationsService) {
        super(http, notification);
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

    editGroupValue(groupId: number, key: string, value: any): Observable<Object> {
        return this.postJson('/api/group/edit', {
            groupId: groupId,
            key: key,
            value: value
        }).map(res => res.json());
    }
}
