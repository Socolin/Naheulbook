import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {NotificationsService} from '../notifications';

import {LoginService} from './login.service';
import {User} from './user.model';
import {MdDialog, MdDialogRef} from '@angular/material';
import {LoginDialogComponent} from "./login-dialog.component";

@Component({
    selector: 'login',
    templateUrl: './login.component.html',
})
export class LoginComponent implements OnInit {
    public user: User;
    public dialogRef: MdDialogRef<LoginDialogComponent>;

    constructor(public dialog: MdDialog
        , private _loginService: LoginService
        , private _router: Router
        , private _notification: NotificationsService) {
    }

    openDialog() {
        this.dialogRef = this.dialog.open(LoginDialogComponent, {
            disableClose: false
        });

        this.dialogRef.afterClosed().subscribe(result => {
            if (result) {
                this.login(result);
            }
            this.dialogRef = null;
        });
    }

    viewProfile() {
        this._router.navigate(['/profile']);
        return false;
    }

    login(method: string) {
        if (method === 'facebook') {
            this._loginService.redirectToFbLogin();
        }
        else if (method === 'google') {
            this._loginService.redirectToGoogleLogin();
        }
        else if (method === 'twitter') {
            this._loginService.redirectToTwitterLogin();
        }
    }

    logout() {
        this._loginService.logout().subscribe(() => {
            this._router.navigate(['']);
            this._notification.info('Déconnexion', 'Vous êtes a present déconnecté');
        });

        return false;
    }

    ngOnInit() {
        this._loginService.loggedUser.subscribe(
            user => {
                this.user = user;
            }
        );
        this._loginService.checkLogged();
    }
}
