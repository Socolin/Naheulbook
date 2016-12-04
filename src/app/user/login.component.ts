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
    public user: User;
    public selectingLoginMethod: boolean;

    constructor(private _loginService: LoginService
        , private _router: Router
        , private _notification: NotificationsService) {
    }

    viewProfile() {
        this._router.navigate(['/profile']);
        return false;
    }

    login(method: string) {
        if (method === 'facebook') {
            this._loginService.redirectToFbLogin();
        }
        else if (method === 'google') {
            this._loginService.redirectToGoogleLogin();
        }
        else if (method === 'twitter') {
            this._loginService.redirectToTwitterLogin();
        }
    }

    closeSelector() {
        this.selectingLoginMethod = false;
    }

    selectLoginMethod() {
        this.selectingLoginMethod = true;
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
