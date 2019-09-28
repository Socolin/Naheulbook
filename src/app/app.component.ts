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
        private readonly themeService: ThemeService,
    ) {
    };

    ngOnInit() {
        this.themeService.updateTheme();
    }
}

