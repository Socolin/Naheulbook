import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, ReplaySubject} from 'rxjs';
import {MasteryGroupResponse, MasteryGroupsResponse} from '../api/responses';

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
}
