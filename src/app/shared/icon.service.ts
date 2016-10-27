import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';

@Injectable()
export class IconService {
    private icons: ReplaySubject<string[]>;

    constructor(private _http: Http) {
    }

    getIcons(): Observable<string[]> {
        if (!this.icons) {
            this.icons = new ReplaySubject<string[]>(1);

            this._http.get('/api/misc/icons')
                .map(res => res.json())
                .subscribe(
                    icons => {
                        this.icons.next(icons);
                        this.icons.complete();
                    },
                    error => {
                        this.icons.error(error);
                    }
                );
        }
        return this.icons;
    }
}
