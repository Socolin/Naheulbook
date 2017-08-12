import {Component, OnInit, ViewChild} from '@angular/core';
import {OverlayRef, Portal} from '@angular/material';

import {LoginService} from '../user';
import {ThemeService} from '../theme.service';

import {NhbkDialogService} from './nhbk-dialog.service';

@Component({
    selector: 'common-nav',
    templateUrl: './common-nav.component.html',
    styleUrls: ['./common-nav.component.scss'],
})
export class CommonNavComponent implements OnInit {
    @ViewChild('themeSelectorDialog')
    public themeSelectorDialog: Portal<any>;
    public themeSelectorOverlayRef: OverlayRef | undefined;

    constructor(private _themeService: ThemeService,
                private _nhbkDialogService: NhbkDialogService,
                public _loginService: LoginService) {
    };

    changeTheme(theme: string) {
        this._themeService.setTheme(theme);
    }

    openThemeSelector() {
        this.themeSelectorOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.themeSelectorDialog);
    }

    closeThemeSelector() {
        if (this.themeSelectorOverlayRef) {
            this.themeSelectorOverlayRef.detach();
            this.themeSelectorOverlayRef = undefined;
        }
    }

    ngOnInit() {
    }
}

