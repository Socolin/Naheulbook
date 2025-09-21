import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import { NavigationStart, Router, RouterLink, RouterOutlet } from '@angular/router';

import { MatSidenav, MatSidenavContainer } from '@angular/material/sidenav';
import {GmModeService} from '../shared';
import {Subscription} from 'rxjs';
import { MatNavList, MatListItem, MatListItemIcon } from '@angular/material/list';
import { MatIcon } from '@angular/material/icon';
import { CommonNavComponent } from '../shared/common-nav.component';
import { MatToolbar, MatToolbarRow } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';
import { AsyncPipe } from '@angular/common';

@Component({
    templateUrl: './home-database.component.html',
    styleUrls: ['./home-database.component.scss'],
    imports: [MatSidenavContainer, MatSidenav, MatNavList, MatListItem, RouterLink, MatIcon, MatListItemIcon, CommonNavComponent, MatToolbar, MatToolbarRow, MatIconButton, RouterOutlet, AsyncPipe]
})
export class HomeDatabaseComponent implements OnInit, OnDestroy {
    @ViewChild('start', {static: true, read: MatSidenav})
    public start: MatSidenav;
    private subscription = new Subscription();

    constructor(
        public readonly router: Router,
        public readonly gmModeService: GmModeService
    ) {
    };

    ngOnInit() {
        this.subscription.add(this.router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this.start.close().then();
            }
        }))
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe();
    }
}

