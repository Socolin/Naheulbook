import {Http, Headers, Response} from '@angular/http';
import {Observable} from 'rxjs/Rx';

export class JsonService {
    constructor(protected _http: Http) {
    }

    postJson(url: string, data: any): Observable<Response> {
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return this._http.post(url, JSON.stringify(data), {
            headers: headers
        });
    }
}
