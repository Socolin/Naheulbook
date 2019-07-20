import {throwError as observableThrowError, Observable} from 'rxjs';
import {ErrorHandler, Injectable, NgZone} from '@angular/core';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {ErrorReportService} from './error-report.service';
import {catchError} from 'rxjs/operators';

import * as Sentry from '@sentry/browser';
import {environment} from '../environments/environment';

Sentry.init({
    dsn: environment.sentryDsn,
    beforeSend(event, hint) {
        console.log(event);
        console.log(hint);
        const processedEvent = { ...event };
        if (hint.originalException && hint.originalException instanceof Error) {
            processedEvent.extra = processedEvent.extra || {};
            for (let key in hint.originalException) {
                if (!hint.originalException.hasOwnProperty(key)) {
                    continue;
                }
                processedEvent.extra['___error.' + key] = hint.originalException[key];
            }
        }

        return processedEvent;
    },
});

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

        super.handleError(error);

        if (error instanceof HttpErrorResponse) {
            if (error.status === 502) {
                return;
            }
        }

        const eventId = Sentry.captureException(error.originalError || error);
        Sentry.showReportDialog({
            eventId,
            lang: 'fr',
            title: 'Échec Critique',
            subtitle: 'Une erreur est survenue, les informations de l\'erreur ont été enregistré pour pouvoir la corrigé.'
        });
    }
}
