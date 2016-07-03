import {Injectable} from '@angular/core';
import {Response, Http} from '@angular/http';
import {Observable} from 'rxjs/Rx';

import {User} from './user.model';

@Injectable()
export class LoginService {
    public loggedUser: User = null;

    constructor(private _http: Http) {
    }

    getLoginToken(): Observable<string> {
        return this._http.get('/api/user/loginToken')
            .map(res => res.json());
    }

    doFBLogin(code: string, loginToken: string, redirectUri: string): Observable<User> {
        let login = this._http.post('/api/user/fblogin', JSON.stringify({
                code: code,
                loginToken: loginToken,
                redirectUri: redirectUri
            }))
            .share()
            .map(res => res.json());
        login.subscribe(user => {
           this.loggedUser = user;
        });

        return login;
    }

    logout() {
        let logout = this._http.get('/api/user/logout')
            .share()
            .map(res => res.json());
        logout.subscribe(() => {
            this.loggedUser = null;
        });

        return logout;
    }

    checkLogged() {
        let logged = this._http.get('/api/user/logged')
            .share()
            .map(res => res.json());
        logged.subscribe(user => {
            this.loggedUser = user;
        });

        return logged;
    }

    updateProfile(profile): Observable<Response> {
        return this._http.post('/api/user/updateProfile', JSON.stringify(profile))
            .map(res => res.json());
    }

    searchUser(filter: string): Observable<Object[]> {
        return this._http.post('/api/user/searchUser', JSON.stringify({filter: filter}))
            .map(res => res.json());
    }
}
