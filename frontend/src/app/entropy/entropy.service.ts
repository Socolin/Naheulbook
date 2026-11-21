import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, ReplaySubject} from 'rxjs';
import {EntropyTablesResponse} from '../api/responses';
import {ApplyEntropyEventRequest} from '../api/requests';

export type EntropyKind = 'mankdebol' | 'debilibeuk';

/** "Dès que vous avez 5 points, vous tirez une calamité / un bienfait" */
export const entropyPointsNeeded = 5;

@Injectable({providedIn: 'root'})
export class EntropyService {
    private cache?: ReplaySubject<EntropyTablesResponse>;

    constructor(private readonly httpClient: HttpClient) {
    }

    getEntropyTables(): Observable<EntropyTablesResponse> {
        if (!this.cache) {
            const subject = new ReplaySubject<EntropyTablesResponse>(1);
            this.cache = subject;

            this.httpClient.get<EntropyTablesResponse>('/api/v2/entropyTables').subscribe({
                next: response => {
                    subject.next(response);
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

    applyEntropyEvent(groupId: number, kind: EntropyKind, request: ApplyEntropyEventRequest): Observable<void> {
        const resource = kind === 'mankdebol' ? 'mankdebolCalamities' : 'debilibeukBlessings';
        return this.httpClient.post<void>(`/api/v2/groups/${groupId}/${resource}`, request);
    }
}
