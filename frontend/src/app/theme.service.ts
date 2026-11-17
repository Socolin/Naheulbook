import {Injectable} from '@angular/core';

export interface ThemeInfo {
    id: string;
    label: string;
}

export const DEFAULT_THEME = 'nhbk-light-theme';

@Injectable({providedIn: 'root'})
export class ThemeService {
    // Classes are defined in theme.scss
    readonly themes: ThemeInfo[] = [
        {id: 'nhbk-light-theme', label: 'Clair 1'},
        {id: 'nhbk-light2-theme', label: 'Clair 2'},
        {id: 'nhbk-dark-theme', label: 'Sombre 1'},
        {id: 'nhbk-dark2-theme', label: 'Sombre 2'},
    ];

    get currentTheme(): string {
        return localStorage.getItem('theme') || DEFAULT_THEME;
    }

    setTheme(theme: string) {
        localStorage.setItem('theme', theme);
        this.updateTheme();
    }

    updateTheme() {
        document.body.className = this.currentTheme;
    }
}
