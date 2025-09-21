import {Injectable} from '@angular/core';

@Injectable({providedIn: 'root'})
export class ThemeService {
    setTheme(theme: string) {
        localStorage.setItem('theme', theme);
        this.updateTheme();
    }

    updateTheme() {
        let body = document.getElementsByTagName('body')[0];
        let theme = localStorage.getItem('theme');
        if (theme) {
            body.className = theme;
        }
        else {
            body.className = '';
        }
    }
}
