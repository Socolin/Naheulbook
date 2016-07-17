import {Component, OnInit} from '@angular/core';
import {Router, ROUTER_DIRECTIVES, NavigationStart} from '@angular/router';
import {HTTP_PROVIDERS} from '@angular/http';
import {SimpleNotificationsComponent, NotificationsService} from './notifications';

import {LoginService, LoginComponent} from './user';
import {EffectService} from './effect';
import {ItemService} from './item';
import {JobService} from './job';
import {OriginService} from './origin';
import {SkillService} from './skill';
import {CharacterService} from './character';
import {User} from './user';
import {LocationService} from './location/location.service';
import {MonsterService} from './monster';

@Component({
    selector: 'index',
    moduleId: module.id,
    templateUrl: 'app.component.html',
    directives: [SimpleNotificationsComponent, LoginComponent, ROUTER_DIRECTIVES],
    providers: [LoginService
        , HTTP_PROVIDERS
        , NotificationsService
        , EffectService
        , ItemService
        , JobService
        , OriginService
        , SkillService
        , CharacterService
        , LocationService
        , MonsterService
    ],
})
export class IndexComponent implements OnInit {
    public loggedUser: User;

    constructor(private _loginService: LoginService
        , private _notifications: NotificationsService
        , private _router: Router) {
        _router.events.subscribe(
            event => {
                if (event instanceof NavigationStart) {
                    if (event.url.lastIndexOf('/create-', 0) === 0) {
                        this.checkIsAdmin();
                    } else if (event.url.lastIndexOf('/edit-', 0) === 0) {
                        this.checkIsAdmin();
                    } else if (event.url.lastIndexOf('/character', 0) === 0) {
                        this.checkIsLogged();
                    }
                }
            });
    }

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

