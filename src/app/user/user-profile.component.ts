import {Component, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications';
import {LoginService} from './login.service';
import {UserInfoResponse} from '../api/responses';

@Component({
    selector: 'user-profile',
    templateUrl: './user-profile.component.html',
    styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {
    public profile?: UserInfoResponse;

    constructor(
        private readonly notification: NotificationsService,
        private readonly loginService: LoginService,
    ) {
    }

    updateProfile() {
        if (!this.profile) {
            return;
        }
        this.loginService.updateProfile(this.profile.id, {displayName: this.profile.displayName}).subscribe(
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
