import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, ReplaySubject} from 'rxjs';
import {MasteryGroupResponse, MasteryGroupsResponse, MasteryResponse} from '../api/responses';
import {AddCharacterMasteryRequest} from '../api/requests';

@Injectable({providedIn: 'root'})
export class MasteryService {
    private cache?: ReplaySubject<MasteryGroupResponse[]>;

    constructor(private readonly httpClient: HttpClient) {
    }

    getMasteryGroups(): Observable<MasteryGroupResponse[]> {
        if (!this.cache) {
            const subject = new ReplaySubject<MasteryGroupResponse[]>(1);
            this.cache = subject;

            this.httpClient.get<MasteryGroupsResponse>('/api/v2/masteryGroups').subscribe({
                next: response => {
                    subject.next(response.masteryGroups);
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

    addCharacterMastery(characterId: number, request: AddCharacterMasteryRequest): Observable<MasteryResponse> {
        return this.httpClient.post<MasteryResponse>(`/api/v2/characters/${characterId}/masteries`, request);
    }

    removeCharacterMastery(characterId: number, masteryId: string): Observable<void> {
        return this.httpClient.delete<void>(`/api/v2/characters/${characterId}/masteries/${masteryId}`);
    }
}
