import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';

import {LoginService} from './login.service';
import {User} from './user.model';
import {MatDialog} from '@angular/material';

@Component({
    selector: 'login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
    public user: User|null;
    public redirectPage: string;
    public moreInfo: boolean;

    constructor(public dialog: MatDialog
        , private _loginService: LoginService
        , private _route: ActivatedRoute) {
    }

    login(method: string) {
        if (method === 'facebook') {
            this._loginService.redirectToFbLogin(this.redirectPage);
        }
        else if (method === 'google') {
            this._loginService.redirectToGoogleLogin(this.redirectPage);
        }
        else if (method === 'twitter') {
            this._loginService.redirectToTwitterLogin(this.redirectPage);
        }
        else if (method === 'live') {
            this._loginService.redirectToLiveLogin(this.redirectPage);
        }
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
