import {Injectable} from '@angular/core';
import {Http} from '@angular/http';

import {NotificationsService} from '../notifications';
import {LoginService} from '../user';
import {JsonService} from './json-service';
import {Observable} from 'rxjs/Observable';
import {Stat} from './stat.model';
import {ReplaySubject} from 'rxjs/ReplaySubject';
import {ItemTemplate} from '../item/item-template.model';

@Injectable()
export class MiscService extends JsonService {
    private stats: ReplaySubject<Stat[]>;

    constructor(http: Http
        , notification: NotificationsService
        , loginService: LoginService) {
        super(http, notification, loginService);
    }

    getStats(): Observable<Stat[]> {
        if (!this.stats) {
            this.stats = new ReplaySubject<Stat[]>(1);

            this._http.get('/api/character/stats')
                .map(res => res.json())
                .subscribe(
                    stats => {
                        this.stats.next(stats);
                    },
                    error => {
                        this.stats.error(error);
                    }
                );
        }
        return this.stats;
    }

    searchItem(filter): Observable<ItemTemplate[]> {
        if (!filter) {
            return Observable.from([]);
        }
        return this.postJson('/api/item/search', {filter: filter})
            .map(res => res.json());
    }
}
