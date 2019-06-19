import {Injectable} from '@angular/core';
import {Http} from '@angular/http';

import {NotificationsService} from '../notifications';
import {LoginService} from '../user';
import {JsonService} from './json-service';
import {Observable} from 'rxjs/Observable';
import {Stat} from './stat.model';
import {ReplaySubject} from 'rxjs/ReplaySubject';
import {ItemTemplate} from '../item-template/item-template.model';
import {God} from './god.model';

@Injectable()
export class MiscService extends JsonService {
    private stats: ReplaySubject<Stat[]>;
    private gods: ReplaySubject<God[]>;

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
                        this.stats.complete();
                    },
                    error => {
                        this.stats.error(error);
                    }
                );
        }
        return this.stats;
    }

    getGods(): Observable<God[]> {
        if (!this.gods) {
            this.gods = new ReplaySubject<God[]>(1);

            this._http.get('/api/v2/gods')
                .map(res => res.json())
                .subscribe(
                    gods => {
                        this.gods.next(gods);
                        this.gods.complete();
                    },
                    error => {
                        this.gods.error(error);
                    }
                );
        }
        return this.gods;
    }

    getGodsByTechName(): Observable<{[techName: string]: God}> {
        return this.getGods().map((gods: God[]) => {
            let godsByTechName = {};
            gods.map(g => godsByTechName[g.techName] = g);
            return godsByTechName;
        });
    }

    searchItem(filter): Observable<ItemTemplate[]> {
        if (!filter) {
            return Observable.from([]);
        }
        return this.postJson('/api/item/search', {filter: filter})
            .map(res => res.json());
    }
}
