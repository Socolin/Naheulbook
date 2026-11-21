import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, ReplaySubject} from 'rxjs';
import {CharacterSpellBookResponse, SpellBookResponse, SpellBooksResponse} from '../api/responses';
import {AddCharacterSpellBookRequest, UpdateCharacterSpellBookRequest} from '../api/requests';

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

    addCharacterSpellBook(characterId: number, request: AddCharacterSpellBookRequest): Observable<CharacterSpellBookResponse> {
        return this.httpClient.post<CharacterSpellBookResponse>(`/api/v2/characters/${characterId}/spellBooks`, request);
    }

    updateCharacterSpellBook(characterId: number, spellBookId: string, request: UpdateCharacterSpellBookRequest): Observable<CharacterSpellBookResponse> {
        return this.httpClient.put<CharacterSpellBookResponse>(`/api/v2/characters/${characterId}/spellBooks/${spellBookId}`, request);
    }

    removeCharacterSpellBook(characterId: number, spellBookId: string): Observable<void> {
        return this.httpClient.delete<void>(`/api/v2/characters/${characterId}/spellBooks/${spellBookId}`);
    }
}
