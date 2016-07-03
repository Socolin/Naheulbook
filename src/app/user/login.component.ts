import {Component} from '@angular/core';
import {Router} from '@angular/router';
import {NotificationsService} from '../notifications';

import {LoginService} from './login.service';

@Component({
    selector: 'login',
    moduleId: module.id,
    templateUrl: 'login.component.html',
})
export class LoginComponent {
    private fbAppKey: string = '780527738745692';

    constructor(private _loginService: LoginService
        , private _router: Router
        , private _notification: NotificationsService) {
    }

    redirectToFbLogin() {
        this._loginService.getLoginToken().subscribe(loginToken => {
            let state = 'facebook: ' + loginToken;
            let redirectUrl = 'https://www.facebook.com/dialog/oauth?client_id=' + this.fbAppKey
                + '&state=' + state
                + '&response_type=code'
                + '&redirect_uri=' + window.location.origin + '/logged';
            window.location.href = redirectUrl;
        });
   }

    viewProfile() {
        this._router.navigate(['/profile']);;
        return false;
    }

    login() {
        this.redirectToFbLogin();
    }

    logout() {
        this._loginService.logout();
        return false;
    }
}
