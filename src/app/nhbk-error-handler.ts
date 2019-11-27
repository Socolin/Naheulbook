import {ErrorHandler, Injectable, NgZone} from '@angular/core';
import {HttpErrorResponse} from '@angular/common/http';
import {ErrorReportService} from './error-report.service';

import * as Sentry from '@sentry/browser';
import {Event, EventHint} from '@sentry/types';
import {environment} from '../environments/environment';

if (environment.production) {
    Sentry.init({
        dsn: environment.sentryDsn,
        beforeSend(event: Event, hint?: EventHint) {
            const processedEvent = {...event};
            if (hint && hint.originalException && hint.originalException instanceof Error) {
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
}

@Injectable()
export class NhbkErrorHandler extends ErrorHandler {
    private count = 0;

    constructor(
        private readonly ngZone: NgZone,
        private readonly errorReportService: ErrorReportService
    ) {
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

        this.ngZone.run(() => {
            this.errorReportService.notify('Une erreur est survenue', error);
        });

        super.handleError(error);

        if (error instanceof HttpErrorResponse) {
            if (error.status === 502) {
                return;
            }
        }

        if (environment.production) {
            try {
                const eventId = Sentry.captureException(error.originalError || error);
                Sentry.showReportDialog({
                    eventId,
                    lang: 'fr',
                    user: {
                        name: 'Nobody',
                        email: 'nobody@nobody.com'
                    },
                    title: 'Échec Critique',
                    subtitle: 'Une erreur est survenue, les informations de l\'erreur ont été enregistré pour pouvoir la corriger.'
                });
            } catch (e) {
                console.error(error);
                console.error(e);
                Sentry.captureException(new Error(`An error occured while logging another error.\n`
                    + ` OriginalError message: ${(error.originalError || error).message}, sentry error: ${e.message}`));
            }
        }
    }
}
