import {Component} from '@angular/core';
import {MatDialogActions, MatDialogClose, MatDialogContent, MatDialogTitle} from '@angular/material/dialog';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatRipple} from '@angular/material/core';
import {ThemeInfo, ThemeService} from '../theme.service';

@Component({
    selector: 'app-theme-selector-dialog',
    templateUrl: './theme-selector-dialog.component.html',
    styleUrls: ['./theme-selector-dialog.component.scss'],
    imports: [MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose, MatButton, MatIcon, MatRipple]
})
export class ThemeSelectorDialogComponent {
    readonly themes: ThemeInfo[];

    constructor(
        private readonly themeService: ThemeService,
    ) {
        this.themes = themeService.themes;
    }

    get currentTheme(): string {
        return this.themeService.currentTheme;
    }

    changeTheme(theme: ThemeInfo) {
        this.themeService.setTheme(theme.id);
    }
}
