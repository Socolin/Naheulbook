import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, ReplaySubject} from 'rxjs';

@Injectable({providedIn: 'root'})
export class IconService {
    private icons: ReplaySubject<string[]>;

    constructor(private httpClient: HttpClient) {
    }

    getIcons(): Observable<string[]> {
        if (!this.icons) {
            this.icons = new ReplaySubject<string[]>(1);

            this.httpClient.get<string[]>('/assets/icons-list.json')
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
