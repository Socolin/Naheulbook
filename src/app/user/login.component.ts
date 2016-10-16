import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {NotificationsService} from '../notifications';

import {LoginService} from './login.service';
import {User} from './user.model';

@Component({
    selector: 'login',
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
            window.location.href = 'https://www.facebook.com/dialog/oauth?client_id=' + authInfos.appKey
                + '&state=' + state
                + '&response_type=code'
                + '&redirect_uri=' + window.location.origin + '/logged';
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
            this._notification.info('Déconnexion', 'Vous êtes a present déconnecté');
        });

        return false;
    }

    ngOnInit() {
        this._loginService.loggedUser.subscribe(
            user => {
                this.user = user;
            }
        );
        this._loginService.checkLogged();
    }
}
