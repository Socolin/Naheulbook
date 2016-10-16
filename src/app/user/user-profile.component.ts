import {Component, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications';
import {LoginService} from './login.service';
import {User} from './user.model';

@Component({
    selector: 'user-profile',
    templateUrl: 'user-profile.component.html'
})
export class UserProfileComponent implements OnInit {
    public profile: User;

    constructor(private _notification: NotificationsService
        , private loginService: LoginService) {
    }

    updateProfile() {
        this.loginService.updateProfile(this.profile).subscribe(
            () => {
                this._notification.success('Succes', 'Profile editer');
            },
            err => {
                console.log(err);
                this._notification.error('Erreur', 'Erreur');
            }
        );
    }

    ngOnInit() {
        this.loginService.loggedUser.subscribe(user => {
            this.profile = user;
        });
    }
}
