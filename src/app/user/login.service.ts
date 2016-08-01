import {Injectable} from '@angular/core';
import {Response, Http} from '@angular/http';
import {Observable, ReplaySubject} from 'rxjs/Rx';

import {User} from './user.model';
import {JsonService} from '../shared/json-service';

@Injectable()
export class LoginService extends JsonService {
    public loggedUser: ReplaySubject<User> = new ReplaySubject<User>(1);

    constructor(http: Http) {
        super(http);
    }

    getLoginToken(app: string): Observable<{loginToken: string, appKey: string}> {
        return this.postJson('/api/user/loginToken', {app: app})
            .map(res => res.json());
    }

    doFBLogin(code: string, loginToken: string, redirectUri: string): Observable<User> {
        let fbLogin = this.postJson('/api/user/fblogin', {
                code: code,
                loginToken: loginToken,
                redirectUri: redirectUri
            })
            .map(res => res.json())
            .share();

        fbLogin.subscribe(user => {
            this.loggedUser.next(user);
        });

        return fbLogin;
    }

    logout(): Observable<User> {
        let logout = this._http.get('/api/user/logout')
            .map(res => res.json())
            .share();

        logout.subscribe(() => {
            this.loggedUser.next(null);
        });

        return logout;
    }

    checkLogged(): Observable<User> {
        let checkLogged = this._http.get('/api/user/logged')
            .map(res => res.json())
            .share();

        checkLogged.subscribe(user => {
            this.loggedUser.next(user);
        });

        return checkLogged;
    }

    updateProfile(profile): Observable <Response> {
        return this.postJson('/api/user/updateProfile', profile)
            .map(res => res.json());
    }

    searchUser(filter: string): Observable <Object[]> {
        return this._http.post('/api/user/searchUser', JSON.stringify({filter: filter}))
            .map(res => res.json());
    }
}
