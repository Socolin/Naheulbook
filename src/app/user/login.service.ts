import {Injectable} from '@angular/core';
import {Response, Http} from '@angular/http';
import {Observable, ReplaySubject} from 'rxjs/Rx';

import {User} from './user.model';
import {JsonService} from '../shared/json-service';
import {NotificationsService} from '../notifications/notifications.service';
import {isNullOrUndefined} from 'util';

@Injectable()
export class LoginService extends JsonService {
    public loggedUser: ReplaySubject<User> = new ReplaySubject<User>(1);
    public currentLoggedUser: User;

    constructor(http: Http
        , notification: NotificationsService) {
        super(http, notification, null);
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
            this.currentLoggedUser = user;
        });

        return fbLogin;
    }

    doGoogleLogin(code: string, loginToken: string, redirectUri: string): Observable<User> {
        let googleLogin = this.postJson('/api/user/googleLogin', {
                code: code,
                loginToken: loginToken,
                redirectUri: redirectUri
            })
            .map(res => res.json())
            .share();

        googleLogin.subscribe(user => {
            this.loggedUser.next(user);
            this.currentLoggedUser = user;
        });

        return googleLogin;
    }

    doLiveLogin(code: string, loginToken: string, redirectUri: string): Observable<User> {
        let liveLogin = this.postJson('/api/user/liveLogin', {
                code: code,
                loginToken: loginToken,
                redirectUri: redirectUri
            })
            .map(res => res.json())
            .share();

        liveLogin.subscribe(user => {
            this.loggedUser.next(user);
            this.currentLoggedUser = user;
        });

        return liveLogin;
    }

    doTwitterLogin(oauthToken: string, oauthVerifier: string): Observable<User> {
        let twitterLogin = this.postJson('/api/user/twitterLogin', {
            oauthToken: oauthToken,
            oauthVerifier: oauthVerifier
        })
            .map(res => res.json())
            .share();

        twitterLogin.subscribe(user => {
            this.loggedUser.next(user);
            this.currentLoggedUser = user;
        });

        return twitterLogin;
    }


    logout(): Observable<User> {
        let logout = this._http.get('/api/user/logout')
            .map(res => res.json())
            .share();

        logout.subscribe(() => {
            this.currentLoggedUser = null;
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
            this.currentLoggedUser = user;
        });

        return checkLogged;
    }

    isLogged(): Observable<boolean> {
        return this.checkLogged().map(user => !isNullOrUndefined(user));
    }


    updateProfile(profile): Observable <Response> {
        return this.postJson('/api/user/updateProfile', profile)
            .map(res => res.json());
    }

    searchUser(filter: string): Observable <Object[]> {
        return this.postJson('/api/user/searchUser', {filter: filter})
            .map(res => res.json());
    }

    redirectToFbLogin(redirectPage: string) {
        localStorage.setItem('redirectPage', redirectPage);
        this.getLoginToken('facebook').subscribe(authInfos => {
            let state = 'facebook:' + authInfos.loginToken;
            window.location.href = 'https://www.facebook.com/dialog/oauth?client_id=' + authInfos.appKey
                + '&state=' + state
                + '&response_type=code'
                + '&redirect_uri=' + window.location.origin + '/logged';
        });
    }

    redirectToGoogleLogin(redirectPage: string) {
        localStorage.setItem('redirectPage', redirectPage);
        this.getLoginToken('google').subscribe(authInfos => {
            let state = 'google:' + authInfos.loginToken;
            window.location.href = 'https://accounts.google.com/o/oauth2/auth?client_id=' + authInfos.appKey
                + '&state=' + state
                + '&scope=profile'
                + '&response_type=code'
                + '&redirect_uri=' + window.location.origin + '/logged';
        });
    }

    redirectToLiveLogin(redirectPage: string) {
        localStorage.setItem('redirectPage', redirectPage);
        this.getLoginToken('live').subscribe(authInfos => {
            let state = 'live:' + authInfos.loginToken;
                window.location.href = 'https://login.live.com/oauth20_authorize.srf?client_id=' + authInfos.appKey
                + '&state=' + state
                + '&scope=wl.signin'
                + '&response_type=code'
                + '&redirect_uri=' + window.location.origin + '/logged';
        });
    }

    redirectToTwitterLogin(redirectPage: string) {
        localStorage.setItem('redirectPage', redirectPage);
        this.getLoginToken('twitter').subscribe(authInfos => {
            window.location.href = 'https://api.twitter.com/oauth/authenticate?oauth_token=' + authInfos.loginToken;
        });
    }
}
