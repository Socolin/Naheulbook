import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';
import {UserAdminResponse} from '../api/responses';
import {SearchUserByIdentifiersRequest} from '../api/requests';

/**
 * Users administration, reserved to super admins (`adminLevel` 2)
 */
@Injectable({providedIn: 'root'})
export class AdminUserService {
    constructor(
        private readonly httpClient: HttpClient
    ) {
    }

    searchByIdentifiers(request: SearchUserByIdentifiersRequest): Observable<UserAdminResponse[]> {
        let params = new HttpParams();
        for (const [key, value] of Object.entries(request)) {
            if (value) {
                params = params.set(key, value);
            }
        }
        return this.httpClient.get<UserAdminResponse[]>('/api/v2/users', {params});
    }

    deleteUser(userId: number): Observable<void> {
        return this.httpClient.delete<void>(`/api/v2/users/${userId}`);
    }
}
