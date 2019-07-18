import {from as observableFrom, Observable, ReplaySubject} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {Stat} from './stat.model';
import {ItemTemplate} from '../item-template';
import {God} from './god.model';

@Injectable()
export class MiscService {
    private stats: ReplaySubject<Stat[]>;
    private gods: ReplaySubject<God[]>;

    constructor(private httpClient: HttpClient) {
    }

    getStats(): Observable<Stat[]> {
        if (!this.stats) {
            this.stats = new ReplaySubject<Stat[]>(1);

            this.httpClient.get<Stat[]>('/api/v2/stats')
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

            this.httpClient.get<God[]>('/api/v2/gods')
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

    getGodsByTechName(): Observable<{ [techName: string]: God }> {
        return this.getGods().pipe(map((gods: God[]) => {
            let godsByTechName = {};
            gods.map(g => godsByTechName[g.techName] = g);
            return godsByTechName;
        }));
    }

    searchItem(filter): Observable<ItemTemplate[]> {
        if (!filter) {
            return observableFrom([]);
        }
        return this.httpClient.get<ItemTemplate[]>('/api/v2/itemTemplates/search?filter=' + encodeURIComponent(filter));
    }

    postError(url, error) {
        return this.httpClient.post(url, error);
    }
}
