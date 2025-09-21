import {Component, OnDestroy, OnInit} from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import {Subscription} from 'rxjs';

import {NotificationsService} from '../notifications';
import {LoginService} from './login.service';

@Component({
    templateUrl: './logged.component.html',
    imports: [RouterLink]
})
export class LoggedComponent implements OnInit, OnDestroy {
    public isInErrorState = false;
    private subscription: Subscription;

    constructor(
        private readonly loginService: LoginService,
        private readonly notification: NotificationsService,
        private readonly router: Router,
    ) {
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

        this.subscription = this.router.routerState.root.queryParams.subscribe(params => {
            if (params.hasOwnProperty('state')) {
                let state = decodeURIComponent(params['state']).split(':', 2);
                let app = state[0];
                let token = state[1];
                if (app === 'facebook') {
                    if (params.hasOwnProperty('code')) {
                        this.loginService.doFBLogin(
                            params['code']
                            , token
                            , window.location.origin + window.location.pathname).subscribe(
                            () => {
                                this.router.navigateByUrl(redirectUrl, {replaceUrl: true, });
                            }, () => {
                                this.isInErrorState = true;
                            }
                        );
                    } else {
                        this.isInErrorState = true;
                        this.notification.error('Échec de l\'authentification', params);
                    }
                } else if (app === 'google') {
                    if (params.hasOwnProperty('code')) {
                        this.loginService.doGoogleLogin(
                            params['code']
                            , token
                            , window.location.origin + window.location.pathname).subscribe(
                            () => {
                                this.router.navigateByUrl(redirectUrl, {replaceUrl: true, });
                            }, () => {
                                this.isInErrorState = true;
                            }
                        );
                    } else {
                        this.isInErrorState = true;
                        this.notification.error('Échec de l\'authentification', params);
                    }
                } else if (app === 'microsoft') {
                    if (params.hasOwnProperty('code')) {
                        this.loginService.doMicrosoftLogin(
                            params['code']
                            , token
                            , window.location.origin + window.location.pathname).subscribe(
                            () => {
                                this.router.navigateByUrl(redirectUrl, {replaceUrl: true, });
                            }, () => {
                                this.isInErrorState = true;
                            }
                        );
                    } else {
                        this.isInErrorState = true;
                        this.notification.error('Échec de l\'authentification', params);
                    }
                } else if (app === 'twitter') {
                    if (params.hasOwnProperty('oauth_token') && params.hasOwnProperty('oauth_verifier')) {
                        this.loginService.doTwitterLogin(params['oauth_token'], params['oauth_verifier']).subscribe(
                            () => {
                                this.router.navigateByUrl(redirectUrl, {replaceUrl: true, });
                            }, () => {
                                this.isInErrorState = true;
                            }
                        );
                    } else {
                        this.isInErrorState = true;
                        this.notification.error('Échec de l\'authentification', params);
                    }
                } else {
                    this.isInErrorState = true;
                    this.notification.error('Invalid app', params);
                }
            } else {
                this.isInErrorState = true;
                this.notification.error('Authentification impossible', params);
            }
        });
    }
}
