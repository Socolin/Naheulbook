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
    public profile?: User;

    constructor(
        private readonly notification: NotificationsService,
        private readonly loginService: LoginService,
    ) {
    }

    updateProfile() {
        if (!this.profile) {
            return;
        }
        this.loginService.updateProfile(this.profile.id, this.profile).subscribe(
            () => {
                this.notification.success('Succes', 'Profile édité');
            }
        );
    }

    linkTo(method: string) {
        if (method === 'facebook') {
            this.loginService.redirectToFbLogin('profile');
        } else if (method === 'google') {
            this.loginService.redirectToGoogleLogin('profile');
        } else if (method === 'twitter') {
            this.loginService.redirectToTwitterLogin('profile');
        } else if (method === 'microsoft') {
            this.loginService.redirectToMicrosoftLogin('profile');
        }
    }

    ngOnInit(): void {
        this.loginService.loggedUser.subscribe(user => {
            this.profile = user;
        });
    }
}
