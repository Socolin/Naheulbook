import {Component, OnInit} from '@angular/core';

import {ThemeService} from './theme.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
    public initialized = false;
    constructor(
        private readonly themeService: ThemeService
    ) {
    };

    ngOnInit() {
        this.themeService.updateTheme();
    }
}

