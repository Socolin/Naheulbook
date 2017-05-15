import {Component, OnInit, ViewChild} from '@angular/core';
import {Router} from '@angular/router';
import {NotificationsService} from '../notifications';

import {MdSidenav} from '@angular/material';
import {User} from '../user/user.model';
import {LoginService} from '../user/login.service';
import {CharacterService} from '../character/character.service';
import {CharacterResume} from '../character/character.model';

@Component({
    templateUrl: './home-player.component.html',
    styleUrls: ['./home-player.component.scss'],
})
export class HomePlayerComponent implements OnInit {
    public characters: CharacterResume[];
    public loggedUser: User;

    @ViewChild('start', {read: MdSidenav})
    public start: MdSidenav;

    constructor(private _characterService: CharacterService
        , private _loginService: LoginService
        , private _notifications: NotificationsService
        , public _router: Router) {

    };

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

        this._characterService.loadList().subscribe(
            characters => {
                this.characters = characters;
            }
        );
    }
}

