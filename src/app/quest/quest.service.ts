import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable} from 'rxjs/Rx';

import {QuestTemplate} from "./quest.model";

@Injectable()
export class QuestService {
    constructor(private _http: Http) {
    }

    createQuestTemplate(quest: QuestTemplate): Observable<QuestTemplate> {
        return this._http.post('/api/quest/createQuestTemplate', JSON.stringify({
            quest: quest
        })).map(res => res.json());
    }

    getQuestList(): Observable < QuestTemplate[] > {
        return this._http.get('/api/quest/listQuest').map(res => res.json());
    }

    searchQuest(name): Observable < QuestTemplate[] > {
        return this._http.post('/api/quest/searchQuest', JSON.stringify({
            name: name
        })).map(res => res.json());
    }
}
