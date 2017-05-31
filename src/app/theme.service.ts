import {Injectable} from '@angular/core';

@Injectable()
export class ThemeService {
    setTheme(theme: string) {
        localStorage.setItem('theme', theme);
        this.updateTheme();
    }

    updateTheme() {
        let body = document.getElementsByTagName('body')[0];
        let theme = localStorage.getItem('theme');
        switch (theme) {
            case 'dark':
                body.className = 'nhbk-dark-theme';
                break;
            default:
                body.className = '';
                break;
        }
    }
}
