import {Http, Headers, Response} from '@angular/http';
import {Observable} from 'rxjs/Rx';
import {NotificationsService} from '../notifications';
import {LoginService} from '../user';

export class JsonService {
    constructor(protected _http: Http
        , private _notification: NotificationsService
        , private _loginService: LoginService) {
    }

    postJson(url: string, data?: any, reportingError?: boolean): Observable<Response> {
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return this._http.post(url, JSON.stringify(data), {
            headers: headers
        })
        .catch((err: any, caught: Observable<any>) => {
            if (reportingError) {
                return;
            }
            if (err.status) {
                if (err.status === 401) {
                    this._notification.error('Erreur', 'Vous devez vous identifier.');
                    if (this._loginService) {
                        this._loginService.loggedUser.next(null);
                    }
                }
                else if (err.status >= 500) {
                    this.postJson('/api/debug/report', err).subscribe();
                    this._notification.error('Erreur', 'Erreur serveur');
                }
                else if (err.status >= 400) {
                    this.postJson('/api/debug/report', err).subscribe();
                    this._notification.error('Erreur', 'Erreur javascript');
                }
                else {
                    this.postJson('/api/debug/report', err).subscribe();
                    this._notification.error('Erreur', 'Erreur inconnu');
                }
            }
            else {
                this.postJson('/api/debug/report', err).subscribe();
                this._notification.error('Erreur', 'Erreur inconnu');
            }
            console.log(err);
            return Observable.throw(err);
        });
    }
}
