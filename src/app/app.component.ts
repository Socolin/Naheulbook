import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {ThemeService} from './theme.service';
import {NotificationsService} from './notifications';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
    constructor(
        private _themeService: ThemeService
        , public _router: Router
        , public notifications: NotificationsService
    ) {
    };

    ngOnInit() {
        this._themeService.updateTheme();
    }
}

