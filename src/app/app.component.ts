import {Component, OnInit, ViewChild} from '@angular/core';
import {Router, NavigationStart} from '@angular/router';
import {NotificationsService} from './notifications';

import {LoginService} from './user';
import {User} from './user';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
    public loggedUser: User;

    constructor(private _loginService: LoginService
        , private _notifications: NotificationsService
        , public _router: Router) {
    };

    checkIsAdmin() {
        if (this.loggedUser) {
            return this.loggedUser.admin;
        }
        let subscription = this._loginService.loggedUser.subscribe(
            user => {
                if (subscription) {
                    subscription.unsubscribe();
                }
                this.loggedUser = user;
                if (user === null) {
                    this._notifications.error('Accès interdit', 'Vous devez vous authentifier');
                    this._router.navigate(['']);
                }
            }
        );
    }

    checkIsLogged() {
        if (this.loggedUser) {
            return true;
        }
        let subscription = this._loginService.loggedUser.subscribe(
            user => {
                subscription.unsubscribe();
                this.loggedUser = user;
                if (user === null) {
                    this._notifications.error('Accès interdit', 'Vous devez vous authentifier');
                    this._router.navigate(['']);
                }
            });
    }

    ngOnInit() {
        this._loginService.loggedUser.subscribe(user => {
            this.loggedUser = user;
        });
    }
}

