import {Component, OnInit, ViewChild} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {LoginService} from '../user';
import {ThemeService} from '../theme.service';

import {NhbkDialogService} from './nhbk-dialog.service';
import {Router} from '@angular/router';
import {GmModeService} from './gm-mode.service';
import {MatDialog} from '@angular/material/dialog';
import {ConfirmGmModeDialogComponent} from './confirm-gm-mode-dialog.component';

@Component({
    selector: 'common-nav',
    templateUrl: './common-nav.component.html',
    styleUrls: ['./common-nav.component.scss'],
})
export class CommonNavComponent implements OnInit {
    @ViewChild('themeSelectorDialog', {static: true})
    public themeSelectorDialog: Portal<any>;
    public themeSelectorOverlayRef: OverlayRef | undefined;

    constructor(
        public readonly loginService: LoginService,
        private readonly nhbkDialogService: NhbkDialogService,
        private readonly themeService: ThemeService,
        private readonly router: Router,
        private readonly dialog: MatDialog,
        public readonly gmModeService: GmModeService,
    ) {
    };

    changeTheme(theme: string) {
        this.themeService.setTheme(theme);
    }

    openThemeSelector() {
        this.themeSelectorOverlayRef = this.nhbkDialogService.openCenteredBackdropDialog(this.themeSelectorDialog);
    }

    closeThemeSelector() {
        if (this.themeSelectorOverlayRef) {
            this.themeSelectorOverlayRef.detach();
            this.themeSelectorOverlayRef = undefined;
        }
    }

    ngOnInit() {
    }

    redirectToLogin() {
        this.router.navigate(['login', document.location.pathname.replace('/', '@')])
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

