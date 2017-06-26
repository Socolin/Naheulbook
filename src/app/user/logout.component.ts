import {Component, OnInit} from '@angular/core';

import {LoginService} from './login.service';
import {Router} from '@angular/router';

@Component({
    template: `logout`,
})
export class LogoutComponent implements OnInit {
    constructor(private _loginService: LoginService
        , private _router: Router) {
    }

    ngOnInit() {
        this._loginService.logout().subscribe(() => {
            this._router.navigateByUrl('/');
        });
    }
}
