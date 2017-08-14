import {ErrorHandler, Injectable} from '@angular/core';
import {Http, Headers, Response} from '@angular/http';
import {Observable} from 'rxjs/Rx';
import {ErrorReportService} from './error-report.service';

@Injectable()
export class NhbkErrorHandler extends ErrorHandler {
    private count = 0;

    constructor(private _http: Http,
                private _errorReportService: ErrorReportService) {
        super(true);
    }

    postJson(url: string, data: any): Observable<Response> {
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');

        let cache = [];
        let json = JSON.stringify(data, function(key, value) {
            if (typeof value === 'object' && value !== null) {
                if (cache.indexOf(value) !== -1) {
                    // Circular reference found, discard key
                    return;
                }
                // Store value in our collection
                cache.push(value);
            }
            return value;
        });
        cache = null;

        return this._http.post(url, json, {
            headers: headers
        })
        .catch((err: any) => {
            console.log(err);
            return Observable.throw(err);
        });
    }

    handleError(error: any) {
        this.count++;
        if (this.count > 10) {
            return;
        }

        this._errorReportService.notify('An error occurred', error);

        this.postJson('/api/debug/report', {error: error}).subscribe(() => {
            console.log('error reported');
        });
        super.handleError(error);
    }
}
