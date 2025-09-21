import {Component, OnInit, ViewChild} from '@angular/core';
import {NavigationStart, Router, RouterLink, RouterOutlet} from '@angular/router';
import {MatSidenav, MatSidenavContainer, MatSidenavContent} from '@angular/material/sidenav';

import {GroupService} from '../group';
import {GroupSummaryResponse} from '../api/responses';
import {MatListItem, MatListItemIcon, MatNavList} from '@angular/material/list';
import {MatIcon} from '@angular/material/icon';
import {CommonNavComponent} from '../shared/common-nav.component';
import {MatToolbar, MatToolbarRow} from '@angular/material/toolbar';
import {MatIconButton} from '@angular/material/button';

@Component({
    templateUrl: './home-gm.component.html',
    styleUrls: ['./home-gm.component.scss'],
    imports: [MatSidenavContainer, MatSidenav, MatNavList, MatListItem, RouterLink, MatIcon, MatListItemIcon, CommonNavComponent, MatSidenavContent, MatToolbar, MatToolbarRow, MatIconButton, RouterOutlet]
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

