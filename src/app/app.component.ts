import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {LoginService} from './user';
import {User} from './user';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
    public loggedUser: User;

    constructor(private _loginService: LoginService
        , public _router: Router) {
    };

    ngOnInit() {
        this._loginService.loggedUser.subscribe(user => {
            this.loggedUser = user;
        });
    }
}

