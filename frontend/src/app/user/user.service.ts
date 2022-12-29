import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {UserAccessTokenResponse, UserAccessTokenResponseWithKey} from '../api/responses';
import {HttpClient} from '@angular/common/http';
import {CreateAccessTokenRequest} from '../api/requests';

@Injectable()
export class UserService {
    constructor(
        private readonly httpClient: HttpClient
    ) {
    }

    getAccessTokens(): Observable<UserAccessTokenResponse[]> {
        return this.httpClient.get<UserAccessTokenResponse[]>(`/api/v2/users/me/accessTokens`);
    }

    createAccessToken(request: CreateAccessTokenRequest): Observable<UserAccessTokenResponseWithKey> {
        return this.httpClient.post<UserAccessTokenResponseWithKey>(`/api/v2/users/me/accessTokens`, request);
    }

    deleteAccessToken(id: string): Observable<any> {
        return this.httpClient.delete<UserAccessTokenResponseWithKey>(`/api/v2/users/me/accessTokens/${id}`);
    }
}
