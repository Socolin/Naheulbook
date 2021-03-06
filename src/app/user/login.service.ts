import {map, retryWhen, share} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {from, Observable, ReplaySubject} from 'rxjs';

import {genericRetryStrategy} from '../shared/rxjs-retry-strategy';
import {AuthenticationInitResponse, JwtResponse, UserInfoResponse, UserSearchResponse} from '../api/responses';
import {UpdateUserRequest} from '../api/requests';

@Injectable()
export class LoginService {
    public loggedUser: ReplaySubject<UserInfoResponse | undefined> = new ReplaySubject<UserInfoResponse>(1);
    public currentLoggedUser?: UserInfoResponse;
    public currentJwt?: string;
    private checkingLoggedUser?: Observable<JwtResponse>;
    private renewTokenTimeout?: any;

    // TODO: Renewing token

    constructor(
        private readonly httpClient: HttpClient,
    ) {
    }

    getLoginToken(app: string): Observable<AuthenticationInitResponse> {
        return this.httpClient.get<AuthenticationInitResponse>(`/api/v2/authentications/${app}/initOAuthAuthentication`);
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

    logout(): Observable<UserInfoResponse> {
        let logout = this.httpClient.get<UserInfoResponse>('/api/v2/users/me/logout').pipe(share());

        logout.subscribe(() => {
            this.currentLoggedUser = undefined;
            this.currentJwt = undefined;
            this.loggedUser.next(undefined);
        });

        return logout;
    }

    checkLogged(): Observable<UserInfoResponse | undefined> {
        if (this.checkingLoggedUser) {
            return this.loggedUser;
        }

        if (this.renewTokenTimeout) {
            clearTimeout(this.renewTokenTimeout);
        }

        this.checkingLoggedUser = this.httpClient.get<JwtResponse>('/api/v2/users/me/jwt').pipe(
            retryWhen(genericRetryStrategy({
                excludedStatusCodes: [401, 400],
                maxRetryAttempts: 1
            })),
            share()
        );

        this.checkingLoggedUser.subscribe(response => {
            this.loggedUser.next(response.userInfo);
            this.currentLoggedUser = response.userInfo;
            this.currentJwt = response.token;
            this.checkingLoggedUser = undefined;
            this.startRenewToken(response.token);
        }, (error) => {
            this.loggedUser.next(undefined);
            this.currentLoggedUser = undefined;
            this.currentJwt = undefined;
            this.checkingLoggedUser = undefined;
            if (!(error instanceof HttpErrorResponse) || error.status !== 401) {
                this.loggedUser.error(error);
            }
        });

        return this.loggedUser;
    }

    isLogged(): Observable<boolean> {
        if (this.currentJwt) {
            return from([true]);
        }
        return this.loggedUser.pipe(map(user => user !== undefined));
    }

    updateProfile(userId: number, changeRequest: UpdateUserRequest): Observable<void> {
        return this.httpClient.patch<void>(`/api/v2/users/${userId}`, changeRequest);
    }

    searchUser(filter: string): Observable<UserSearchResponse[]> {
        return this.httpClient.post<UserSearchResponse[]>('/api/v2/users/search', {filter: filter});
    }

    redirectToFbLogin(redirectPage: string, errorCb?: (error) => void) {
        localStorage.setItem('redirectPage', redirectPage);
        this.getLoginToken('facebook').subscribe(authInfos => {
            let state = 'facebook:' + authInfos.loginToken;
            window.location.href = 'https://www.facebook.com/dialog/oauth?client_id=' + authInfos.appKey
                + '&state=' + state
                + '&response_type=code'
                + '&redirect_uri=' + window.location.origin + '/logged';
        }, errorCb);
    }

    redirectToGoogleLogin(redirectPage: string, errorCb?: (error) => void) {
        localStorage.setItem('redirectPage', redirectPage);
        this.getLoginToken('google').subscribe(authInfos => {
            let state = 'google:' + authInfos.loginToken;
            window.location.href = 'https://accounts.google.com/o/oauth2/auth?client_id=' + authInfos.appKey
                + '&state=' + state
                + '&scope=profile'
                + '&response_type=code'
                + '&redirect_uri=' + window.location.origin + '/logged';
        }, errorCb);
    }

    redirectToMicrosoftLogin(redirectPage: string, errorCb?: (error) => void) {
        localStorage.setItem('redirectPage', redirectPage);
        this.getLoginToken('microsoft').subscribe(authInfos => {
            let state = 'microsoft:' + authInfos.loginToken;
            window.location.href = 'https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=' + authInfos.appKey
                + '&state=' + state
                + '&scope=openid,User.Read'
                + '&response_type=code'
                + '&redirect_uri=' + window.location.origin + '/logged';
        }, errorCb);
    }

    redirectToTwitterLogin(redirectPage: string, errorCb?: (error) => void) {
        localStorage.setItem('redirectPage', redirectPage);
        this.getLoginToken('twitter').subscribe(authInfos => {
            window.location.href = 'https://api.twitter.com/oauth/authenticate?oauth_token=' + authInfos.loginToken;
        }, errorCb);
    }

    private startRenewToken(token: string) {
        const tokenPayload = JSON.parse(atob(token.split('.')[1]));
        this.renewTokenTimeout = setTimeout(() => {
            this.checkLogged();
        }, ((tokenPayload.exp * 1000) - (new Date().getTime())) / 2);
    }
}
