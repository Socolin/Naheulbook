import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {LoginService} from './login.service';
import {NhbkMatDialog} from '../material-workaround';
import {combineLatest, forkJoin, Subscription} from 'rxjs';
import {share} from 'rxjs/operators';

@Component({
    selector: 'login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit, OnDestroy {
    public redirectPage: string;
    public moreInfo: boolean;
    public loading = false;
    public subscription: Subscription = new Subscription();

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly loginService: LoginService,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
    ) {
    }

    onLoginError(error: any): void {
        this.loading = false;
        throw error;
    }

    login(method: string) {
        this.loading = true;
        if (method === 'facebook') {
            this.loginService.redirectToFbLogin(this.redirectPage, this.onLoginError.bind(this));
        } else if (method === 'google') {
            this.loginService.redirectToGoogleLogin(this.redirectPage, this.onLoginError.bind(this));
        } else if (method === 'twitter') {
            this.loginService.redirectToTwitterLogin(this.redirectPage, this.onLoginError.bind(this));
        } else if (method === 'microsoft') {
            this.loginService.redirectToMicrosoftLogin(this.redirectPage, this.onLoginError.bind(this));
        }
    }

    goToHomePage() {
        this.router.navigateByUrl('/');
    }

    ngOnInit() {
        this.loading = true;
        this.subscription.add(this.route.params.subscribe(params => {
            this.redirectPage = decodeURIComponent(params['redirect']);
        }));

        this.subscription.add(combineLatest([
            this.route.params,
            this.loginService.checkLogged(),
        ]).subscribe(([params, user]) => {
            if (user) {
                this.loading = true;
                this.router.navigateByUrl(decodeURIComponent(params['redirect']), {replaceUrl: true});
            } else {
                this.loading = false;
            }
        }, () => {
            this.loading = false;
        }));
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe();
    }
}
