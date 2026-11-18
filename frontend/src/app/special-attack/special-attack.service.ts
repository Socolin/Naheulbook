import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, ReplaySubject} from 'rxjs';
import {CharacterSpecialAttackResponse, SpecialAttackGroupResponse, SpecialAttackGroupsResponse, UseCharacterSpecialAttackResponse} from '../api/responses';
import {AddCharacterSpecialAttackRequest, UpdateCharacterSpecialAttackRequest, UseCharacterSpecialAttackRequest} from '../api/requests';

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

    addCharacterSpecialAttack(characterId: number, request: AddCharacterSpecialAttackRequest): Observable<CharacterSpecialAttackResponse> {
        return this.httpClient.post<CharacterSpecialAttackResponse>(`/api/v2/characters/${characterId}/specialAttacks`, request);
    }

    removeCharacterSpecialAttack(characterId: number, specialAttackId: string): Observable<void> {
        return this.httpClient.delete<void>(`/api/v2/characters/${characterId}/specialAttacks/${specialAttackId}`);
    }

    useCharacterSpecialAttack(characterId: number, specialAttackId: string, request: UseCharacterSpecialAttackRequest): Observable<UseCharacterSpecialAttackResponse> {
        return this.httpClient.post<UseCharacterSpecialAttackResponse>(`/api/v2/characters/${characterId}/specialAttacks/${specialAttackId}/use`, request);
    }

    updateCharacterSpecialAttack(characterId: number, specialAttackId: string, request: UpdateCharacterSpecialAttackRequest): Observable<CharacterSpecialAttackResponse> {
        return this.httpClient.put<CharacterSpecialAttackResponse>(`/api/v2/characters/${characterId}/specialAttacks/${specialAttackId}`, request);
    }
}
