import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {NotificationsService} from '../notifications';

import {LoginService} from './login.service';
import {User} from './user.model';

@Component({
    selector: 'login',
    moduleId: module.id,
    templateUrl: 'login.component.html',
})
export class LoginComponent implements OnInit {
    private user: User;

    constructor(private _loginService: LoginService
        , private _router: Router
        , private _notification: NotificationsService) {
    }

    redirectToFbLogin() {
        this._loginService.getLoginToken('facebook').subscribe(authInfos => {
            let state = 'facebook:' + authInfos.loginToken;
            let redirectUrl = 'https://www.facebook.com/dialog/oauth?client_id=' + authInfos.appKey
                + '&state=' + state
                + '&response_type=code'
                + '&redirect_uri=' + window.location.origin + '/logged';
            window.location.href = redirectUrl;
        });
    }

    viewProfile() {
        this._router.navigate(['/profile']);
        return false;
    }

    login() {
        this.redirectToFbLogin();
    }

    logout() {
        this._loginService.logout().subscribe(() => {
            this._router.navigate(['']);
        });

        return false;
    }

    ngOnInit() {
        this._loginService.loggedUser.subscribe(
            user => {
                this.user = user;
            }
        );
    }
}
