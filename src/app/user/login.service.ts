import {share, map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, ReplaySubject} from 'rxjs';

import {User} from './user.model';
import {isNullOrUndefined} from 'util';

@Injectable()
export class LoginService {
    public loggedUser: ReplaySubject<User | null> = new ReplaySubject<User>(1);
    public currentLoggedUser?: User;

    constructor(private httpClient: HttpClient) {
    }

    getLoginToken(app: string): Observable<{ loginToken: string, appKey: string }> {
        return this.httpClient.post<{ loginToken: string, appKey: string }>('/api/user/loginToken', {app: app});
    }

    doFBLogin(code: string, loginToken: string, redirectUri: string): Observable<User> {
        let fbLogin = this.httpClient.post<User>('/api/user/fblogin', {
            code: code,
            loginToken: loginToken,
            redirectUri: redirectUri
        }).pipe(
            share()
        );

        fbLogin.subscribe(user => {
            this.loggedUser.next(user);
            this.currentLoggedUser = user;
        });

        return fbLogin;
    }

    doGoogleLogin(code: string, loginToken: string, redirectUri: string): Observable<User> {
        let googleLogin = this.httpClient.post<User>('/api/user/googleLogin', {
            code: code,
            loginToken: loginToken,
            redirectUri: redirectUri
        }).pipe(
            share()
        );

        googleLogin.subscribe(user => {
            this.loggedUser.next(user);
            this.currentLoggedUser = user;
        });

        return googleLogin;
    }

    doLiveLogin(code: string, loginToken: string, redirectUri: string): Observable<User> {
        let liveLogin = this.httpClient.post<User>('/api/user/liveLogin', {
            code: code,
            loginToken: loginToken,
            redirectUri: redirectUri
        }).pipe(
            share()
        );

        liveLogin.subscribe(user => {
            this.loggedUser.next(user);
            this.currentLoggedUser = user;
        });

        return liveLogin;
    }

    doTwitterLogin(oauthToken: string, oauthVerifier: string): Observable<User> {
        let twitterLogin = this.httpClient.post<User>('/api/user/twitterLogin', {
            oauthToken: oauthToken,
            oauthVerifier: oauthVerifier
        }).pipe(
            share()
        );

        twitterLogin.subscribe(user => {
            this.loggedUser.next(user);
            this.currentLoggedUser = user;
        });

        return twitterLogin;
    }


    logout(): Observable<User> {
        let logout = this.httpClient.get<User>('/api/user/logout').pipe(share());

        logout.subscribe(() => {
            this.currentLoggedUser = undefined;
            this.loggedUser.next(null);
        });

        return logout;
    }

    checkLogged(): Observable<User> {
        let checkLogged = this.httpClient.get<User>('/api/v2/users/me').pipe(
            map(res => {
                return res;
            }),
            share()
        );

        checkLogged.subscribe(user => {
            this.loggedUser.next(user);
            this.currentLoggedUser = user;
        });

        return checkLogged;
    }

    isLogged(): Observable<boolean> {
        return this.checkLogged().pipe(map(user => !isNullOrUndefined(user)));
    }


    updateProfile(profile): Observable<any> {
        return this.httpClient.post<any>('/api/user/updateProfile', profile);
    }

    searchUser(filter: string): Observable<Object[]> {
        return this.httpClient.post<Object[]>('/api/user/searchUser', {filter: filter});
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
