import {Injectable} from '@angular/core';
import {Observable, ReplaySubject} from 'rxjs';
import {AptitudeGroupResponse, AptitudeGroupsResponse, CharacterAddAptitudeResponse} from '../api/responses';
import {HttpClient} from '@angular/common/http';
import {AddCharacterAptitudeRequest, UpdateCharacterAptitudeRequest} from '../api/requests';

@Injectable({providedIn: 'root'})
export class AptitudeService {
    private aptitudeGroupList: ReplaySubject<AptitudeGroupsResponse>;
    private aptitudeGroupCache: Map<string, ReplaySubject<AptitudeGroupResponse>> = new Map<string, ReplaySubject<AptitudeGroupResponse>>();

    constructor(private httpClient: HttpClient) {
    }

    getAptitudeGroups(): Observable<AptitudeGroupsResponse> {
        if (!this.aptitudeGroupList) {
            this.aptitudeGroupList = new ReplaySubject<AptitudeGroupsResponse>(1);

            this.httpClient.get<AptitudeGroupsResponse>('/api/v2/aptitudeGroups').subscribe(
                response => {
                    this.aptitudeGroupList.next(response);
                    this.aptitudeGroupList.complete();
                },
                error => {
                    this.aptitudeGroupList.error(error);
                }
            );
        }
        return this.aptitudeGroupList;
    }

    getAptitudeGroup(aptitudeGroupId: string): Observable<AptitudeGroupResponse> {
        let aptitudeGroupSubject = this.aptitudeGroupCache.get(aptitudeGroupId)
        if (aptitudeGroupSubject) {
            return aptitudeGroupSubject;
        }

        aptitudeGroupSubject = new ReplaySubject<AptitudeGroupResponse>(1);
        this.aptitudeGroupCache.set(aptitudeGroupId, aptitudeGroupSubject);

        this.httpClient.get<AptitudeGroupResponse>(`/api/v2/aptitudeGroups/${aptitudeGroupId}`).subscribe(
            response => {
                aptitudeGroupSubject.next(response);
                aptitudeGroupSubject.complete();
            },
            error => {
                aptitudeGroupSubject.error(error);
            }
        );

        return aptitudeGroupSubject;
    }

    addCharacterAptitude(
        characterId: number,
        request: AddCharacterAptitudeRequest
    ): Observable<CharacterAddAptitudeResponse> {
        return this.httpClient.post<CharacterAddAptitudeResponse>(`/api/v2/characters/${characterId}/aptitudes`, request)
    }

    removeCharacterAptitude(
        characterId: number,
        aptitudeId: string,
    ) {
        return this.httpClient.delete(`/api/v2/characters/${characterId}/aptitudes/${aptitudeId}`)
    }

    updateCharacterAptitude(
        characterId: number,
        aptitudeId: string,
        request: UpdateCharacterAptitudeRequest
    ) {
        return this.httpClient.put<CharacterAddAptitudeResponse>(`/api/v2/characters/${characterId}/aptitudes/${aptitudeId}`, request)
    }
}
