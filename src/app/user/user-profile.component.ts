import {Component, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications';
import {LoginService} from './login.service';
import {UserInfoResponse} from '../api/responses';
import {MatCheckboxChange} from '@angular/material/checkbox';
import {NhbkMatDialog} from '../material-workaround';
import {EnableShowInSearchComponent, EnableShowInSearchResult} from './enable-show-in-search.component';

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
        private readonly dialog: NhbkMatDialog
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

    toggleVisibilityInSearch($event: MatCheckboxChange) {
        if (!this.profile) {
            return;
        }

        if ($event.checked) {
            const dialogRef = this.dialog.open<EnableShowInSearchComponent, unknown, EnableShowInSearchResult>(EnableShowInSearchComponent);
            dialogRef.afterClosed().subscribe((result) => {
                if (!this.profile) {
                    return;
                }
                if (!result) {
                    this.profile!.showInSearch = false;
                    return;
                }

                this.loginService.updateProfile(this.profile.id, {
                    showInSearchFor: result.durationInSeconds
                }).subscribe();
            })
        } else {
            this.loginService.updateProfile(this.profile.id, {
                showInSearchFor: 0
            }).subscribe(() => {
                if (!this.profile) {
                    return;
                }
                this.profile.showInSearch = false;
            });

        }
    }
}
