import {NgModule, ErrorHandler, APP_INITIALIZER, Injectable} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {RouterModule} from '@angular/router';
import {HAMMER_GESTURE_CONFIG} from '@angular/platform-browser';

import {NhbkMaterialModule} from './nhbk-material.module';
import {AppComponent} from './app.component';
import {routes} from './app.routes';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';

import {HomeModule} from './home/home.module';
import {CharacterModule} from './character/character.module';
import {EffectModule} from './effect/effect.module';
import {GroupModule} from './group/group.module';
import {ItemTemplateModule} from './item-template/item-template.module';
import {JobModule} from './job/job.module';
import {LocationModule} from './location/location.module';
import {MonsterModule} from './monster/monster.module';
import {NotificationsModule} from './notifications/notifications.module';
import {OriginModule} from './origin/origin.module';
import {QuestModule} from './quest/quest.module';
import {SharedModule} from './shared/shared.module';
import {SkillModule} from './skill/skill.module';
import {UserModule} from './user/user.module';

import {DateModule} from './date/date.module';
import {NhbkErrorHandler} from './nhbk-error-handler';

import 'hammerjs';
import {CustomHammerConfig} from './hammer-js-config';

import {PlayerHomeModule} from './home-player/home-player.module';
import {GmHomeModule} from './home-gm/home-gm.module';
import {DatabaseHomeModule} from './home-database/home-database.module';
import {ThemeService} from './theme.service';
import {WebsocketModule} from './websocket/websocket.module';
import {ErrorReportService} from './error-report.service';
import {AuthenticationInterceptor} from './user/authentication.interceptor';
import {LoginService} from './user';

@NgModule({
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        RouterModule.forRoot(routes),
        NhbkMaterialModule,
        HttpClientModule,
        HomeModule,
        DatabaseHomeModule,
        PlayerHomeModule,
        GmHomeModule,
        CharacterModule,
        EffectModule,
        GroupModule,
        ItemTemplateModule,
        JobModule,
        LocationModule,
        MonsterModule,
        NotificationsModule,
        OriginModule,
        QuestModule,
        SharedModule,
        SkillModule,
        UserModule,
        DateModule,
        WebsocketModule,
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

function onAppInit(loginService: LoginService): () => Promise<any> {
    return () => new Promise((resolve, reject) => {
        loginService.checkLogged().subscribe(
            () => resolve(),
            () => reject()
        );
    });
}
