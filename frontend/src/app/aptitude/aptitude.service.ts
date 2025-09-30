import {Injectable} from '@angular/core';
import {Observable, ReplaySubject} from 'rxjs';
import {AptitudeGroupResponse, AptitudeGroupsResponse} from '../api/responses/aptitude-response';
import {HttpClient} from '@angular/common/http';

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
        if (!this.aptitudeGroupCache.has(aptitudeGroupId)) {
            let aptitudeGroupSubject = new ReplaySubject<AptitudeGroupResponse>(1);
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
        }
        return this.aptitudeGroupCache.get(aptitudeGroupId);
    }

}
