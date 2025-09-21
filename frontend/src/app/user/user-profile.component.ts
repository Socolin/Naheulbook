import {Component, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications';
import {LoginService} from './login.service';
import {UserAccessTokenResponse, UserInfoResponse} from '../api/responses';
import { MatCheckboxChange, MatCheckbox } from '@angular/material/checkbox';
import {NhbkMatDialog} from '../material-workaround';
import {EnableShowInSearchComponent, EnableShowInSearchResult} from './enable-show-in-search.component';
import {UserService} from './user.service';
import {PromptDialogComponent, PromptDialogData, PromptDialogResult} from '../shared';
import {MatSnackBar} from '@angular/material/snack-bar';
import {Clipboard} from '@angular/cdk/clipboard';
import { MatSidenavContainer } from '@angular/material/sidenav';
import { MatToolbar } from '@angular/material/toolbar';
import { RouterLink } from '@angular/router';
import { MatIcon } from '@angular/material/icon';
import { MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardActions } from '@angular/material/card';
import { MatFormField, MatHint } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatButton, MatIconButton } from '@angular/material/button';
import { MatMenuTrigger, MatMenu, MatMenuItem } from '@angular/material/menu';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { DatePipe } from '@angular/common';

@Component({
    selector: 'user-profile',
    templateUrl: './user-profile.component.html',
    styleUrls: ['./user-profile.component.scss'],
    imports: [MatSidenavContainer, MatToolbar, RouterLink, MatIcon, MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatFormField, MatInput, FormsModule, MatHint, MatCheckbox, MatCardActions, MatButton, MatIconButton, MatMenuTrigger, MatMenu, MatMenuItem, MatProgressSpinner, DatePipe]
})
export class UserProfileComponent implements OnInit {
    public profile?: UserInfoResponse;
    public accessTokens?: UserAccessTokenResponse[];

    constructor(
        private readonly notification: NotificationsService,
        private readonly loginService: LoginService,
        private readonly userService: UserService,
        private readonly dialog: NhbkMatDialog,
        private readonly clipboard: Clipboard,
        private readonly snackBar: MatSnackBar
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
        this.userService.getAccessTokens().subscribe(accessTokens => {
            this.accessTokens = accessTokens.sort((a, b) => a.dateCreated.localeCompare(b.dateCreated))
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

    openCreateAccessTokenDialog() {
        const dialogRef = this.dialog.open<PromptDialogComponent, PromptDialogData, PromptDialogResult>(PromptDialogComponent, {
            data: {
                confirmText: 'CRÉER',
                cancelText: 'ANNULER',
                placeholder: 'Nom',
                title: 'Nom du token (utile seulement pour vous, pour l\'identifier dans la liste)',
            }
        });
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            if (!result.text) {
                return;
            }
            this.userService.createAccessToken({name: result.text}).subscribe(accessToken => {
                this.accessTokens!.push(accessToken);
            });
        })
    }

    deleteAccessToken(id: string) {
        this.userService.deleteAccessToken(id).subscribe(() => {
            let idx = this.accessTokens?.findIndex(x => x.id === id);
            if (idx !== undefined && idx >= 0) {
                this.accessTokens?.splice(idx, 1);
            }
        });
    }

    copyKeyToClipboard(key: string) {
        if (this.clipboard.copy(key)) {
            this.snackBar.open('Clé copié dans le presse papier', undefined, {
                duration: 5000
            });
        }
    }
}
