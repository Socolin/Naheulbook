import {throwError as observableThrowError, Observable} from 'rxjs';
import {ErrorHandler, Injectable, NgZone} from '@angular/core';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {ErrorReportService} from './error-report.service';
import {catchError} from 'rxjs/operators';

@Injectable()
export class NhbkErrorHandler extends ErrorHandler {
    private count = 0;

    constructor(private httpClient: HttpClient,
                private _ngZone: NgZone,
                private _errorReportService: ErrorReportService) {
        super();
    }

    postJson(url: string, data: any): Observable<any> {
        let cache: any[] = [];
        let json = JSON.stringify(data, function (key, value) {
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

        return this.httpClient.post(url, json)
            .pipe(catchError((err: any) => {
                console.log(err);
                return observableThrowError(err);
            }));
    }

    handleError(error: any) {
        if (error instanceof HttpErrorResponse) {
            if (error.status === 401) {
                super.handleError(error);
                return;
            }
        }

        this.count++;
        if (this.count > 10) {
            return;
        }

        this._ngZone.run(() => {
            this._errorReportService.notify('An error occurred', error);
        });

        this.postJson('/api/debug/report', {
            message: error.toString(),
            stacktrace: error.stack,
            url: window.location.toString()
        }).subscribe(() => {
            console.log('error reported');
        });
        super.handleError(error);
    }
}
