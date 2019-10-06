import {APP_INITIALIZER, ErrorHandler, NgModule} from '@angular/core';
import {BrowserModule, HAMMER_GESTURE_CONFIG} from '@angular/platform-browser';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {RouterModule} from '@angular/router';

import {NhbkMaterialModule} from './nhbk-material.module';
import {AppComponent} from './app.component';
import {routes} from './app.routes';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';

import {HomeModule} from './home/home.module';
import {UserModule} from './user/user.module';
import {NhbkErrorHandler} from './nhbk-error-handler';

import 'hammerjs';
import {CustomHammerConfig} from './hammer-js-config';
import {ThemeService} from './theme.service';
import {ErrorReportService} from './error-report.service';
import {AuthenticationInterceptor} from './user/authentication.interceptor';
import {NotificationsModule} from './notifications/notifications.module';
import {LoginService} from './user';

@NgModule({
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        RouterModule.forRoot(routes),
        NhbkMaterialModule,
        HttpClientModule,
        HomeModule,
        UserModule,
        NotificationsModule
    ],
    declarations: [
        AppComponent
    ],
    bootstrap: [
        AppComponent
    ],
    providers: [
        ThemeService,
        ErrorReportService,
        {
            provide: ErrorHandler,
            useClass: NhbkErrorHandler
        },
        {
            provide: HAMMER_GESTURE_CONFIG,
            useClass: CustomHammerConfig
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthenticationInterceptor,
            multi: true
        },
        {
            provide: APP_INITIALIZER,
            useFactory: onAppInit,
            multi: true,
            deps: [LoginService]
        },
    ]
})
export class AppModule {
}

export function onAppInit(loginService: LoginService): () => Promise<any> {
    return () => new Promise((resolve, reject) => {
        loginService.checkLogged().subscribe(
            () => resolve(),
            () => reject()
        );
    });
}
