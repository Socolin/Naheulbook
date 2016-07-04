import {Injectable} from '@angular/core';
import {Response, Http} from '@angular/http';
import {Observable, ReplaySubject} from 'rxjs/Rx';

import {User} from './user.model';

@Injectable()
export class LoginService {
    private internalLoggedUser: ReplaySubject<User> = new ReplaySubject<User>(1);
    public loggedUser: Observable<User>;

    constructor(private _http: Http) {
        this.loggedUser = this.internalLoggedUser.share();
    }

    getLoginToken(app: string): Observable<{loginToken: string, appKey: string}> {
        return this._http.post('/api/user/loginToken', JSON.stringify({app: app}))
            .map(res => res.json());
    }

    doFBLogin(code: string, loginToken: string, redirectUri: string): Observable<User> {
        this._http.post('/api/user/fblogin', JSON.stringify({
                code: code,
                loginToken: loginToken,
                redirectUri: redirectUri
            }))
            .map(res => res.json())
            .subscribe(user => {
                this.internalLoggedUser.next(user);
            });

        return this.loggedUser;
    }

    logout() {
        let logout = this._http.get('/api/user/logout')
            .map(res => res.json())
            .share();

        logout.subscribe(() => {
            this.internalLoggedUser.next(null);
        });

        return logout;
    }

    checkLogged() {
        let checkLogged = this._http.get('/api/user/logged')
            .map(res => res.json())
            .share();

        checkLogged.subscribe(user => {
            this.internalLoggedUser.next(user);
        });

        return checkLogged;
    }

    updateProfile(profile): Observable < Response > {
        return this._http.post('/api/user/updateProfile', JSON.stringify(profile))
            .map(res => res.json());
    }

    searchUser(filter: string): Observable < Object[] > {
        return this._http.post('/api/user/searchUser', JSON.stringify({filter: filter}))
            .map(res => res.json());
    }
}
