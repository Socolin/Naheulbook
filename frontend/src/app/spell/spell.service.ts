import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, ReplaySubject} from 'rxjs';
import {SpellBookResponse, SpellBooksResponse} from '../api/responses';

@Injectable({providedIn: 'root'})
export class SpellService {
    private spellBooks?: ReplaySubject<SpellBookResponse[]>;

    constructor(private readonly httpClient: HttpClient) {
    }

    getSpellBooks(): Observable<SpellBookResponse[]> {
        if (!this.spellBooks) {
            const subject = new ReplaySubject<SpellBookResponse[]>(1);
            this.spellBooks = subject;

            this.httpClient.get<SpellBooksResponse>('/api/v2/spellBooks').subscribe({
                next: response => {
                    subject.next(response.spellBooks);
                    subject.complete();
                },
                error: error => {
                    this.spellBooks = undefined;
                    subject.error(error);
                }
            });
        }
        return this.spellBooks;
    }
}
