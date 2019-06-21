import {Component, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications';
import {LoginService} from './login.service';
import {User} from './user.model';

@Component({
    selector: 'user-profile',
    templateUrl: './user-profile.component.html',
    styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {
    public profile: User|null;

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

    linkTo(method: string) {
        if (method === 'facebook') {
            this.loginService.redirectToFbLogin('profile');
        }
        else if (method === 'google') {
            this.loginService.redirectToGoogleLogin('profile');
        }
        else if (method === 'twitter') {
            this.loginService.redirectToTwitterLogin('profile');
        }
        else if (method === 'microsoft') {
            this.loginService.redirectToMicrosoftLogin('profile');
        }
    }

    ngOnInit(): void {
        this.loginService.loggedUser.subscribe(user => {
            this.profile = user;
        });
    }
}
