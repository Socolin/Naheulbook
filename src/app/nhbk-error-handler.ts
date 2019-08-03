import {ErrorHandler, Injectable, NgZone} from '@angular/core';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {ErrorReportService} from './error-report.service';

import * as Sentry from '@sentry/browser';
import {environment} from '../environments/environment';

Sentry.init({
    dsn: environment.sentryDsn,
    beforeSend(event, hint) {
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
            this._errorReportService.notify('Une erreur est survenue', error);
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
