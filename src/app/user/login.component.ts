import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {LoginService} from './login.service';
import {User} from './user.model';
import {MatDialog} from '@angular/material';
import {ErrorReportService} from '../error-report.service';

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

    constructor(public dialog: MatDialog
        , private _loginService: LoginService
        , private _route: ActivatedRoute
        , private router: Router
    ) {
    }

    onLoginError(error: any): void {
        this.loading = false;
        throw error;
    }

    login(method: string) {
        this.loading = true;
        if (method === 'facebook') {
            this._loginService.redirectToFbLogin(this.redirectPage, this.onLoginError.bind(this));
        } else if (method === 'google') {
            this._loginService.redirectToGoogleLogin(this.redirectPage, this.onLoginError.bind(this));
        } else if (method === 'twitter') {
            this._loginService.redirectToTwitterLogin(this.redirectPage, this.onLoginError.bind(this));
        } else if (method === 'microsoft') {
            this._loginService.redirectToMicrosoftLogin(this.redirectPage, this.onLoginError.bind(this));
        }
    }

    goToHomePage() {
        this.router.navigateByUrl('/');
    }

    ngOnInit() {
        this._route.params.subscribe(params => {
            this.redirectPage = params['redirect'];
            this.redirectPage = this.redirectPage.replace('@', '/');
        });
        this._loginService.loggedUser.subscribe(
            user => {
                this.user = user;
            }
        );
        this._loginService.checkLogged();
    }
}
