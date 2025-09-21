import { ErrorHandler, NgModule, inject, provideAppInitializer } from '@angular/core';
import { BrowserModule, HAMMER_GESTURE_CONFIG } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';

import { NhbkMaterialModule } from './nhbk-material.module';
import { AppComponent } from './app.component';
import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { HomeModule } from './home/home.module';
import { UserModule } from './user/user.module';
import { NhbkErrorHandler } from './nhbk-error-handler';

import { CustomHammerConfig } from './hammer-js-config';
import { ThemeService } from './theme.service';
import { ErrorReportService } from './error-report.service';
import { AuthenticationInterceptor } from './user/authentication.interceptor';
import { NotificationsModule } from './notifications/notifications.module';
import { LoginService } from './user';
import { QuickCommandModule } from './quick-command/quick-command.module';

@NgModule({ declarations: [
        AppComponent
    ],
    bootstrap: [
        AppComponent
    ], imports: [BrowserModule,
        BrowserAnimationsModule,
        RouterModule.forRoot(routes, {}),
        NhbkMaterialModule,
        HomeModule,
        UserModule,
        NotificationsModule,
        QuickCommandModule], providers: [
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
        provideAppInitializer(() => {
        const initializerFn = (onAppInit)(inject(LoginService));
        return initializerFn();
      }),
        provideHttpClient(withInterceptorsFromDi()),
    ] })
export class AppModule {
}

export function onAppInit(loginService: LoginService): () => Promise<void> {
    return () => new Promise((resolve, reject) => {
        loginService.checkLogged().subscribe(
            () => resolve(),
            () => reject()
        );
    });
}
