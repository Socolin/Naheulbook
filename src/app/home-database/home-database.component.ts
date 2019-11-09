import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {NavigationStart, Router} from '@angular/router';

import {MatSidenav} from '@angular/material';
import {GmModeService} from '../shared';
import {Subscription} from 'rxjs';

@Component({
    templateUrl: './home-database.component.html',
    styleUrls: ['./home-database.component.scss'],
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

