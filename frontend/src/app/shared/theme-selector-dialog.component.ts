import {Component} from '@angular/core';
import {ThemeService} from '../theme.service';
import { MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatButton } from '@angular/material/button';

@Component({
    selector: 'app-theme-selector-dialog',
    templateUrl: './theme-selector-dialog.component.html',
    styleUrls: ['./theme-selector-dialog.component.scss'],
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, MatButton, MatDialogActions, MatDialogClose]
})
export class ThemeSelectorDialogComponent {

    constructor(
        private readonly themeService: ThemeService,
    ) {
    }

    changeTheme(theme: string) {
        this.themeService.setTheme(theme);
    }

}
