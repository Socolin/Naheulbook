import './polyfills';
import {enableProdMode, ErrorHandler, importProvidersFrom, inject, provideAppInitializer} from '@angular/core';
import {environment} from './environments/environment';

import {ThemeService} from './app/theme.service';
import {ErrorReportService} from './app/error-report.service';
import {NhbkErrorHandler} from './app/nhbk-error-handler';
import {bootstrapApplication, BrowserModule} from '@angular/platform-browser';
import {HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';
import {AuthenticationInterceptor} from './app/user/authentication.interceptor';
import {LoginService} from './app/user';
import {provideAnimations} from '@angular/platform-browser/animations';
import {provideRouter} from '@angular/router';
import {AppComponent, routes} from './app';

if (environment.production) {
    enableProdMode();
}

bootstrapApplication(AppComponent, {
    providers: [
        importProvidersFrom(BrowserModule),
        ThemeService,
        ErrorReportService,
        {
            provide: ErrorHandler,
            useClass: NhbkErrorHandler
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthenticationInterceptor,
            multi: true
        },
        provideAppInitializer(() => {
            const initializerFn = (onAppInit)(inject(LoginService));
            return initializerFn();
        }),
        provideHttpClient(withInterceptorsFromDi()),
        provideAnimations(),
        provideRouter(routes)
    ]
});

export function onAppInit(loginService: LoginService): () => Promise<void> {
    return () => new Promise((resolve, reject) => {
        loginService.checkLogged().subscribe(
            () => resolve(),
            () => reject()
        );
    });
}
