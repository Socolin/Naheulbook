import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {LoginService} from './user';
import {ThemeService} from './theme.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
    constructor(private _themeService: ThemeService
        , public _router: Router) {
    };

    ngOnInit() {
        this._themeService.updateTheme();
    }
}

