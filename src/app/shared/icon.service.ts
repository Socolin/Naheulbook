
import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ReplaySubject, Observable} from 'rxjs';

@Injectable()
export class IconService {
    private icons: ReplaySubject<string[]>;

    constructor(private httpClient: HttpClient) {
    }

    getIcons(): Observable<string[]> {
        if (!this.icons) {
            this.icons = new ReplaySubject<string[]>(1);

            this.httpClient.get<string[]>('/api/misc/icons')
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
