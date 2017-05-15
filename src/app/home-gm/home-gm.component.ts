import {Component, OnInit, ViewChild} from '@angular/core';
import {NavigationStart, Router} from '@angular/router';
import {NotificationsService} from '../notifications';

import {MdSidenav} from '@angular/material';
import {CharacterService} from '../character/character.service';

@Component({
    templateUrl: './home-gm.component.html',
    styleUrls: ['./home-gm.component.scss'],
})
export class HomeGmComponent implements OnInit {
    public groups: Object[];

    @ViewChild('start', {read: MdSidenav})
    public start: MdSidenav;

    constructor(private _characterService: CharacterService
        , private _notifications: NotificationsService
        , public _router: Router) {
        this._router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this.start.close().then();
            }
        })
    };

    loadGroups() {
        this._characterService.listGroups().subscribe(
            res => {
                this.groups = res.slice(0, 5);
            },
            err => {
                console.log(err);
                this._notifications.error('Erreur', 'Erreur serveur');
            }
        );
    }

    ngOnInit() {
        this.loadGroups();
    }
}

