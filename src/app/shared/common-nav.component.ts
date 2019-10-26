import {Component, OnInit, ViewChild} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {LoginService} from '../user';
import {ThemeService} from '../theme.service';

import {NhbkDialogService} from './nhbk-dialog.service';
import {Router} from '@angular/router';

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
}

