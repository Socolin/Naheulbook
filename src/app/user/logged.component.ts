import {Component, OnInit, OnDestroy} from '@angular/core';
import {ActivatedRoute, ActivatedRouteSnapshot, Router} from '@angular/router';
import {Subscription} from 'rxjs';

import {NotificationsService} from '../notifications';
import {LoginService} from './login.service';

@Component({
    templateUrl: './logged.component.html',
})
export class LoggedComponent implements OnInit, OnDestroy {
    private isInErrorState = false;
    private subscription: Subscription;

    constructor(private _loginService: LoginService
        , private _notification: NotificationsService
        , private _router: Router) {
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }

    ngOnInit() {
        let redirectUrl: string;
        if (localStorage.getItem('redirectPage')) {
            redirectUrl = '/' + localStorage.getItem('redirectPage');
            localStorage.removeItem('redirectPage');
        } else {
            redirectUrl = '/';
        }

        this.subscription = this._router.routerState.root.queryParams.subscribe(params => {
            if (params.hasOwnProperty('state')) {
                let state = decodeURIComponent(params['state']).split(':', 2);
                let app = state[0];
                let token = state[1];
                if (app === 'facebook') {
                    if (params.hasOwnProperty('code')) {
                        this._loginService.doFBLogin(
                            params['code']
                            , token
                            , window.location.origin + window.location.pathname).subscribe(
                            () => {
                                this._router.navigateByUrl(redirectUrl);
                            }, () => {
                                this.isInErrorState = true;
                            }
                        );
                    } else if (params.hasOwnProperty('error')) {
                        this._notification.error('Authentification', 'Erreur: ' + params['error_reason']);
                    } else {
                        console.log('Missing error', params);
                    }
                } else if (app === 'google') {
                    if (params.hasOwnProperty('code')) {
                        this._loginService.doGoogleLogin(
                            params['code']
                            , token
                            , window.location.origin + window.location.pathname).subscribe(
                            () => {
                                this._router.navigateByUrl(redirectUrl);
                            }, () => {
                                this.isInErrorState = true;
                            }
                        );
                    } else if (params.hasOwnProperty('error')) {
                        this._notification.error('Authentification', 'Erreur: ' + params['error_reason']);
                    } else {
                        console.log('Missing error', params);
                    }
                } else if (app === 'microsoft') {
                    if (params.hasOwnProperty('code')) {
                        this._loginService.doMicrosoftLogin(
                            params['code']
                            , token
                            , window.location.origin + window.location.pathname).subscribe(
                            () => {
                                this._router.navigateByUrl(redirectUrl);
                            }, () => {
                                this.isInErrorState = true;
                            }
                        );
                    } else if (params.hasOwnProperty('error')) {
                        this._notification.error('Authentification', 'Erreur: ' + params['error_description']);
                    } else {
                        console.log('Missing error', params);
                    }
                } else if (app === 'twitter') {
                    if (params.hasOwnProperty('oauth_token') && params.hasOwnProperty('oauth_verifier')) {
                        this._loginService.doTwitterLogin(params['oauth_token'], params['oauth_verifier']).subscribe(
                            () => {
                                this._router.navigateByUrl(redirectUrl);
                            }, () => {
                                this.isInErrorState = true;
                            }
                        );
                    } else if (params.hasOwnProperty('error')) {
                        this._notification.error('Authentification', 'Erreur: ' + params['error_reason']);
                    } else {
                        console.log('Missing error', params);
                    }
                } else {
                    this.isInErrorState = true;
                    this._notification.error('Error', 'Invalid app', params);
                }
            }
        });
    }
}
