import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {LoginService} from './login.service';
import {User} from './user.model';
import {MatDialog} from '@angular/material';

@Component({
    selector: 'login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
    public user: User | null;
    public redirectPage: string;
    public moreInfo: boolean;
    public loading = false;

    constructor(
        private readonly dialog: MatDialog,
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
        this.route.params.subscribe(params => {
            this.redirectPage = params['redirect'];
            this.redirectPage = this.redirectPage.replace('@', '/');
        });
        this.loginService.loggedUser.subscribe(
            user => {
                this.user = user;
            }
        );
        this.loginService.checkLogged();
    }
}
