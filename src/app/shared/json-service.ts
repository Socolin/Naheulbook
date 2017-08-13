import {Http, Headers, Response} from '@angular/http';
import {Observable} from 'rxjs/Rx';
import {NotificationsService} from '../notifications';
import {LoginService} from '../user';
import {ObservableInput} from 'rxjs/Observable';

export class JsonService {
    constructor(protected _http: Http
        , private _notification: NotificationsService
        , private _loginService: LoginService | null) {
    }

    postJson(url: string, data?: any, reportingError?: boolean): Observable<Response> {
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return this._http.post(url, JSON.stringify(data), {
            headers: headers
        });
    }
}
