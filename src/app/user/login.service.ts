import {share, map, retry, retryWhen, delay, take} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {from, Observable, ReplaySubject} from 'rxjs';

import {JwtResponse, User} from './user.model';
import {genericRetryStrategy} from '../shared/rxjs-retry-strategy';

@Injectable()
export class LoginService {
    public loggedUser: ReplaySubject<User | null> = new ReplaySubject<User>(1);
    public currentLoggedUser?: User;
    public currentJwt?: string;
    private checkingLoggedUser?: Observable<JwtResponse | undefined>;

    // TODO: Renewing token

    constructor(private httpClient: HttpClient) {
    }

    getLoginToken(app: string): Observable<{ loginToken: string, appKey: string }> {
        return this.httpClient.get<{ loginToken: string, appKey: string }>(`/api/v2/authentications/${app}/initOAuthAuthentication`);
    }

    doFBLogin(code: string, loginToken: string, redirectUri: string): Observable<JwtResponse> {
        let fbLogin = this.httpClient.post<JwtResponse>('/api/v2/authentications/facebook/completeAuthentication', {
            code: code,
            loginToken: loginToken,
            redirectUri: redirectUri
        }).pipe(
            share()
        );

        fbLogin.subscribe(user => {
            this.loggedUser.next(user.userInfo);
            this.currentLoggedUser = user.userInfo;
            this.currentJwt = user.token;
        });

        return fbLogin;
    }

    doGoogleLogin(code: string, loginToken: string, redirectUri: string): Observable<JwtResponse> {
        let googleLogin = this.httpClient.post<JwtResponse>('/api/v2/authentications/google/completeAuthentication', {
            code: code,
            loginToken: loginToken,
            redirectUri: redirectUri
        }).pipe(
            share()
        );

        googleLogin.subscribe(user => {
            this.loggedUser.next(user.userInfo);
            this.currentLoggedUser = user.userInfo;
            this.currentJwt = user.token;
        });

        return googleLogin;
    }

    doMicrosoftLogin(code: string, loginToken: string, redirectUri: string): Observable<JwtResponse> {
        let microsoftLogin = this.httpClient.post<JwtResponse>('/api/v2/authentications/microsoft/completeAuthentication', {
            code: code,
            loginToken: loginToken,
            redirectUri: redirectUri
        }).pipe(
            share()
        );

        microsoftLogin.subscribe(user => {
            this.loggedUser.next(user.userInfo);
            this.currentLoggedUser = user.userInfo;
            this.currentJwt = user.token;
        });

        return microsoftLogin;
    }

    doTwitterLogin(oauthToken: string, oauthVerifier: string): Observable<JwtResponse> {
        let twitterLogin = this.httpClient.post<JwtResponse>('/api/v2/authentications/twitter/completeAuthentication', {
            oauthToken: oauthToken,
            oauthVerifier: oauthVerifier
        }).pipe(
            share()
        );

        twitterLogin.subscribe(user => {
            this.loggedUser.next(user.userInfo);
            this.currentLoggedUser = user.userInfo;
            this.currentJwt = user.token;
        });

        return twitterLogin;
    }

    logout(): Observable<User> {
        let logout = this.httpClient.get<User>('/api/v2/users/me/logout').pipe(share());

        logout.subscribe(() => {
            this.currentLoggedUser = undefined;
            this.currentJwt = undefined;
            this.loggedUser.next(null);
        });

        return logout;
    }

    checkLogged(): Observable<User> {
        if (this.checkingLoggedUser) {
            return this.loggedUser;
        }

        this.checkingLoggedUser = this.httpClient.get<JwtResponse>('/api/v2/users/me/jwt').pipe(
            retryWhen(genericRetryStrategy({
                excludedStatusCodes: [401, 400]
            })),
            share()
        );

        this.checkingLoggedUser.subscribe(response => {
            this.loggedUser.next(response.userInfo);
            this.currentLoggedUser = response.userInfo;
            this.currentJwt = response.token;
            this.checkingLoggedUser = undefined;
        }, () => {
            this.loggedUser.next(undefined);
            this.currentLoggedUser = undefined;
            this.currentJwt = undefined;
            this.checkingLoggedUser = undefined;
        });

        return this.loggedUser;
    }

    isLogged(): Observable<boolean> {
        if (this.currentJwt) {
            return from([true]);
        }
        return this.checkLogged().pipe(map(user => user !== undefined));
    }


    updateProfile(userId: number, changeRequest: {displayName: string}): Observable<void> {
        return this.httpClient.patch<void>(`/api/v2/users/${userId}`, changeRequest);
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

    redirectToMicrosoftLogin(redirectPage: string) {
        localStorage.setItem('redirectPage', redirectPage);
        this.getLoginToken('microsoft').subscribe(authInfos => {
            let state = 'microsoft:' + authInfos.loginToken;
            window.location.href = 'https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=' + authInfos.appKey
                + '&state=' + state
                + '&scope=openid,User.Read'
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
