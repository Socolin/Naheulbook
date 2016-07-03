import {Component, OnInit} from '@angular/core';
import {Router, ROUTER_DIRECTIVES} from '@angular/router'
import {HTTP_PROVIDERS} from '@angular/http';
import {SimpleNotificationsComponent, NotificationsService} from './notifications'

import {LoginService, LoginComponent} from './user';
import {EffectService} from './effect';
import {ItemService} from './item';
import {JobService} from './job';
import {OriginService} from './origin';
import {SkillService} from './skill';
import {CharacterService} from './character';

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
    ],
})
export class IndexComponent implements OnInit {
    constructor(private loginService: LoginService
        , private router: Router) {
        router.events.subscribe(
            event => {
                if (event.url.lastIndexOf('create-', 0) === 0) {

                } else if (event.url.lastIndexOf('edit-', 0) === 0) {

                } else if (event.url.lastIndexOf('character', 0) === 0) {

                }
            });
    }

    ngOnInit() {
        this.loginService.checkLogged().subscribe(() => {
        });
    }
}

