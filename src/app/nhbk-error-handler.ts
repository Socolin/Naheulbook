import {ErrorHandler, Injectable} from '@angular/core';
import {Http, Headers, Response} from '@angular/http';
import {Observable} from 'rxjs/Rx';

@Injectable()
class NhbkErrorHandler extends ErrorHandler {

    constructor(private _http: Http) {
        super(true);
    }

    postJson(url: string, data: any): Observable<Response> {
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        return this._http.post(url, JSON.stringify(data), {
            headers: headers
        })
            .catch((err: any) => {
                console.log(err);
                return Observable.throw(err);
            });
    }

    handleError(error: any) {
        this.postJson('/api/debug/report', {error: "" + error}).subscribe(() => {
            console.log("error reported");
        });
        super.handleError(error);
    }
}

export default NhbkErrorHandler;
