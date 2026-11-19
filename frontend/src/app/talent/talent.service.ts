import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, ReplaySubject} from 'rxjs';
import {TalentGroupResponse, TalentGroupsResponse, TalentResponse} from '../api/responses';
import {AddCharacterTalentRequest} from '../api/requests';

@Injectable({providedIn: 'root'})
export class TalentService {
    private cache?: ReplaySubject<TalentGroupResponse[]>;

    constructor(private readonly httpClient: HttpClient) {
    }

    getTalentGroups(): Observable<TalentGroupResponse[]> {
        if (!this.cache) {
            const subject = new ReplaySubject<TalentGroupResponse[]>(1);
            this.cache = subject;

            this.httpClient.get<TalentGroupsResponse>('/api/v2/talentGroups').subscribe({
                next: response => {
                    subject.next(response.talentGroups);
                    subject.complete();
                },
                error: error => {
                    this.cache = undefined;
                    subject.error(error);
                }
            });
        }
        return this.cache;
    }

    addCharacterTalent(characterId: number, request: AddCharacterTalentRequest): Observable<TalentResponse> {
        return this.httpClient.post<TalentResponse>(`/api/v2/characters/${characterId}/talents`, request);
    }

    removeCharacterTalent(characterId: number, talentId: string): Observable<void> {
        return this.httpClient.delete<void>(`/api/v2/characters/${characterId}/talents/${talentId}`);
    }
}
