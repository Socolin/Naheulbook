import {Component, OnInit, ChangeDetectionStrategy} from '@angular/core';

import {LoginService} from './login.service';
import {Router} from '@angular/router';

@Component({ changeDetection: ChangeDetectionStrategy.Eager,
 template: `logout` })
export class LogoutComponent implements OnInit {
    constructor(
        private readonly loginService: LoginService,
        private readonly router: Router,
    ) {
    }

    ngOnInit() {
        this.loginService.logout().subscribe(() => {
            this.router.navigateByUrl('/');
        });
    }
}
