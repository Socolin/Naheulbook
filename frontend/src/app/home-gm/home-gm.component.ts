import {Component, OnInit, ViewChild} from '@angular/core';
import {NavigationStart, Router} from '@angular/router';
import { MatSidenav } from '@angular/material/sidenav';

import {NotificationsService} from '../notifications';

import {GroupService} from '../group';
import {GroupSummaryResponse} from '../api/responses';

@Component({
    templateUrl: './home-gm.component.html',
    styleUrls: ['./home-gm.component.scss'],
    standalone: false
})
export class HomeGmComponent implements OnInit {
    public groups: GroupSummaryResponse[];

    @ViewChild('start', {static: true, read: MatSidenav})
    public start: MatSidenav;

    constructor(
        private readonly groupService: GroupService,
        private readonly router: Router,
    ) {
        this.router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this.start.close().then();
            }
        })
    };

    loadGroups() {
        this.groupService.listGroups().subscribe(
            res => {
                this.groups = res.slice(0, 5);
            }
        );
    }

    ngOnInit() {
        this.loadGroups();
    }
}

