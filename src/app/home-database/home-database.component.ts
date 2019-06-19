import {Component, OnInit, ViewChild} from '@angular/core';
import {NavigationStart, Router} from '@angular/router';

import {MatSidenav} from '@angular/material';
import {LoginService} from '../user';
import {ThemeService} from '../theme.service';

@Component({
    templateUrl: './home-database.component.html',
    styleUrls: ['./home-database.component.scss'],
})
export class HomeDatabaseComponent implements OnInit {
    @ViewChild('start', {static: true, read: MatSidenav})
    public start: MatSidenav;

    constructor(public _router: Router) {
        this._router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this.start.close().then();
            }
        })
    };

    ngOnInit() {
    }
}

