import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable} from 'rxjs/Rx';

import {MonsterTemplate} from "./monster.model";
import {JsonService} from '../shared/json-service';

@Injectable()
export class MonsterService extends JsonService {
    constructor(http: Http) {
        super(http);
    }

    getMonsterList(): Observable<MonsterTemplate[]> {
        return this._http.get('/api/monster/listMonster').map(res => res.json());
    }

    searchMonster(name): Observable<MonsterTemplate[]> {
        return this.postJson('/api/monster/searchMonster', {
            name: name
        }).map(res => res.json());
    }

}
