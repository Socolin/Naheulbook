import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';

import {QuestTemplate} from './quest.model';

@Injectable({providedIn: 'root'})
export class QuestService {
    constructor(private httpClient: HttpClient) {
    }

    createQuestTemplate(quest: QuestTemplate): Observable<QuestTemplate> {
        return this.httpClient.post<QuestTemplate>('/api/quest/createQuestTemplate', {
            quest: quest
        });
    }

    getQuestList(): Observable<QuestTemplate[]> {
        return this.httpClient.get<QuestTemplate[]>('/api/quest/listQuest');
    }

    searchQuest(name): Observable<QuestTemplate[]> {
        return this.httpClient.post<QuestTemplate[]>('/api/quest/searchQuest', {
            name: name
        });
    }
}
