import {Http, Headers, Response} from '@angular/http';
import {Observable} from 'rxjs/Rx';
import {NotificationsService} from '../notifications/notifications.service';
import {LoginService} from "../user/login.service";

export class JsonService {
    constructor(protected _http: Http
        , private _notification: NotificationsService
        , private _loginService: LoginService) {
    }

    postJson(url: string, data: any): Observable<Response> {
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return this._http.post(url, JSON.stringify(data), {
            headers: headers
        })
        .catch((err: any, caught: Observable<any>) => {
            if (err.status) {
                if (err.status === 401) {
                    this._notification.error("Erreur", "Vous devez vous identifier.");
                    if (this._loginService) {
                        this._loginService.loggedUser.next(null);
                    }
                }
                else if (err.status >= 500) {
                    this._notification.error("Erreur", "Erreur serveur");
                }
                else if (err.status >= 400) {
                    this._notification.error("Erreur", "Erreur javascript");
                }
                else {
                    this._notification.error("Erreur", "Erreur inconnu");
                }
            }
            else {
                this._notification.error("Erreur", "Erreur inconnu");
            }
            console.log(err);
            return Observable.throw(err);
        });
    }
}
