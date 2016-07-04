import {Injectable} from '@angular/core';
import {Response, Http} from '@angular/http';
import {Observable, ReplaySubject} from 'rxjs/Rx';

import {User} from './user.model';

@Injectable()
export class LoginService {
    public loggedUser: ReplaySubject<User> = new ReplaySubject<User>(1);

    constructor(private _http: Http) {
    }

    getLoginToken(app: string): Observable<{loginToken: string, appKey: string}> {
        return this._http.post('/api/user/loginToken', JSON.stringify({app: app}))
            .map(res => res.json());
    }

    doFBLogin(code: string, loginToken: string, redirectUri: string): Observable<User> {
        let fbLogin = this._http.post('/api/user/fblogin', JSON.stringify({
                code: code,
                loginToken: loginToken,
                redirectUri: redirectUri
            }))
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
        return this._http.post('/api/user/updateProfile', JSON.stringify(profile))
            .map(res => res.json());
    }

    searchUser(filter: string): Observable <Object[]> {
        return this._http.post('/api/user/searchUser', JSON.stringify({filter: filter}))
            .map(res => res.json());
    }
}
