import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable, ReplaySubject} from 'rxjs/Rx';

import {Origin} from "./origin.model";

@Injectable()
export class OriginService {
    private origins: ReplaySubject<Origin[]>;

    constructor(private _http: Http) {
    }

    getOriginList(): Observable<Origin[]> {
        if (!this.origins || this.origins.isUnsubscribed) {
            this.origins = new ReplaySubject<Origin[]>(1);

            this._http.get('/api/origin/list')
                .map(res => res.json())
                .subscribe(
                    origins => {
                        this.origins.next(origins);
                    },
                    error => {
                        this.origins.error(error);
                    }
                );
        }
        return this.origins;
    }
}
