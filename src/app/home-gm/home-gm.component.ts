import {Component, OnInit, ViewChild} from '@angular/core';
import {NavigationStart, Router} from '@angular/router';
import {MatSidenav} from '@angular/material';

import {NotificationsService} from '../notifications';

import {GroupService} from '../group';

@Component({
    templateUrl: './home-gm.component.html',
    styleUrls: ['./home-gm.component.scss'],
})
export class HomeGmComponent implements OnInit {
    public groups: Object[];

    @ViewChild('start', {static: true, read: MatSidenav})
    public start: MatSidenav;

    constructor(private _groupService: GroupService
        , private _notifications: NotificationsService
        , public _router: Router) {
        this._router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this.start.close().then();
            }
        })
    };

    loadGroups() {
        this._groupService.listGroups().subscribe(
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

