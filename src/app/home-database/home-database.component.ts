import {Component, OnInit, ViewChild} from '@angular/core';
import {NavigationStart, Router} from '@angular/router';

import {MdSidenav} from '@angular/material';

@Component({
    templateUrl: './home-database.component.html',
    styleUrls: ['./home-database.component.scss'],
})
export class HomeDatabaseComponent implements OnInit {
    @ViewChild('start', {read: MdSidenav})
    public start: MdSidenav;

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

