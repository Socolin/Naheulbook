import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, ReplaySubject} from 'rxjs';
import {SpecialAttackGroupResponse, SpecialAttackGroupsResponse} from '../api/responses';

@Injectable({providedIn: 'root'})
export class SpecialAttackService {
    private specialAttackGroups?: ReplaySubject<SpecialAttackGroupResponse[]>;

    constructor(private readonly httpClient: HttpClient) {
    }

    getSpecialAttackGroups(): Observable<SpecialAttackGroupResponse[]> {
        if (!this.specialAttackGroups) {
            const subject = new ReplaySubject<SpecialAttackGroupResponse[]>(1);
            this.specialAttackGroups = subject;

            this.httpClient.get<SpecialAttackGroupsResponse>('/api/v2/specialAttackGroups').subscribe({
                next: response => {
                    subject.next(response.specialAttackGroups);
                    subject.complete();
                },
                error: error => {
                    this.specialAttackGroups = undefined;
                    subject.error(error);
                }
            });
        }
        return this.specialAttackGroups;
    }
}
