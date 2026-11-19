import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, ReplaySubject} from 'rxjs';
import {HandicapResponse, HandicapsResponse} from '../api/responses';

@Injectable({providedIn: 'root'})
export class HandicapService {
    private cache?: ReplaySubject<HandicapResponse[]>;

    constructor(private readonly httpClient: HttpClient) {
    }

    getHandicaps(): Observable<HandicapResponse[]> {
        if (!this.cache) {
            const subject = new ReplaySubject<HandicapResponse[]>(1);
            this.cache = subject;

            this.httpClient.get<HandicapsResponse>('/api/v2/handicaps').subscribe({
                next: response => {
                    subject.next(response.handicaps);
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
