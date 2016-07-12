import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable} from 'rxjs/Rx'

import {HistoryEntry} from "../shared";
import {Monster} from "../monster";

@Injectable()
export class GroupService {
    constructor(private _http: Http) {
    }

    createMonster(groupId: number, monster): Observable<Monster> {
        return this._http.post('/api/group/createMonster', JSON.stringify({
            monster: monster,
            groupId: groupId
        })).map(res => res.json());
    }

    updateMonster(monsterId: number, fieldName: string, value: any): Observable<Monster> {
        return this._http.post('/api/group/updateMonster', JSON.stringify({
            fieldName: fieldName,
            value: value,
            monsterId: monsterId
        })).map(res => res.json());
    }

    killMonster(monsterId: number): Observable<Monster> {
        return this._http.post('/api/group/killMonster', JSON.stringify({
            monsterId: monsterId
        })).map(res => res.json());
    }

    loadHistory(groupId: number, page: number): Observable<HistoryEntry[]> {
        return this._http.post('/api/group/history', JSON.stringify({
            groupId: groupId,
            page: page
        })).map(res => res.json());
    }

    addLog(groupId: number, info: string, is_gm: boolean): Observable<Object> {
        return this._http.post('/api/group/addLog', JSON.stringify({
            groupId: groupId,
            is_gm: is_gm,
            info: info
        })).map(res => res.json());
    }
}
