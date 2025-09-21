import {Component} from '@angular/core';
import {ThemeService} from '../theme.service';

@Component({
    selector: 'app-theme-selector-dialog',
    templateUrl: './theme-selector-dialog.component.html',
    styleUrls: ['./theme-selector-dialog.component.scss'],
    standalone: false
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
