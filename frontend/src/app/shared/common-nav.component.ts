import {Component, OnInit} from '@angular/core';

import {LoginService} from '../user';
import { Router, RouterLink } from '@angular/router';
import {GmModeService} from './gm-mode.service';
import {ConfirmGmModeDialogComponent} from './confirm-gm-mode-dialog.component';
import {NhbkMatDialog} from '../material-workaround';
import {ThemeSelectorDialogComponent} from './theme-selector-dialog.component';
import { MatNavList, MatListItem, MatListItemIcon } from '@angular/material/list';
import { MatSlideToggle } from '@angular/material/slide-toggle';
import { FormsModule } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { AsyncPipe } from '@angular/common';

@Component({
    selector: 'common-nav',
    templateUrl: './common-nav.component.html',
    styleUrls: ['./common-nav.component.scss'],
    imports: [MatNavList, MatSlideToggle, FormsModule, MatListItem, RouterLink, MatIcon, MatListItemIcon, AsyncPipe]
})
export class CommonNavComponent implements OnInit {
    constructor(
        public readonly loginService: LoginService,
        private readonly router: Router,
        private readonly dialog: NhbkMatDialog,
        public readonly gmModeService: GmModeService,
    ) {
    };

    openThemeSelector() {
        this.dialog.open(ThemeSelectorDialogComponent);
    }

    ngOnInit() {
    }

    redirectToLogin() {
        this.router.navigate(['login', encodeURIComponent(document.location.pathname)]);
    }

    toggleGmMode() {
        if (this.gmModeService.gmModeSnapshot) {
            this.gmModeService.setGmMode(false);
        } else {
            const dialogRef = this.dialog.open(ConfirmGmModeDialogComponent, {
                autoFocus: false
            });

            dialogRef.afterClosed().subscribe((result) => {
                if (!result) {
                    return;
                }

                this.gmModeService.setGmMode(true);
            });
        }
    }
}

