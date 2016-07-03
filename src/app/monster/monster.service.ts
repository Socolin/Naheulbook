import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable} from 'rxjs/Rx';

import {MonsterTemplate} from "./monster.model";

@Injectable()
export class MonsterService {
    constructor(private _http: Http) {
    }

    getMonsterList(): Observable<MonsterTemplate[]> {
        return this._http.get('/api/monster/listMonster').map(res => res.json());
    }

    searchMonster(name): Observable<MonsterTemplate[]> {
        return this._http.post('/api/monster/searchMonster', JSON.stringify({
            name: name
        })).map(res => res.json());
    }

}
