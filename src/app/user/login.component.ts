import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';

import {LoginService} from './login.service';
import {User} from './user.model';
import {MdDialog} from '@angular/material';

@Component({
    selector: 'login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
    public user: User;
    public redirectPage: string;

    constructor(public dialog: MdDialog
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
    }

    ngOnInit() {
        this._route.params.subscribe(params => {
            this.redirectPage = params['redirect'];
        });
        this._loginService.loggedUser.subscribe(
            user => {
                this.user = user;
            }
        );
        this._loginService.checkLogged();
    }
}
