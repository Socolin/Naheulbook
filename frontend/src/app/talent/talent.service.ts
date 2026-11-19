import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, ReplaySubject} from 'rxjs';
import {TalentGroupResponse, TalentGroupsResponse} from '../api/responses';

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
}
