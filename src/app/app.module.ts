import {NgModule, ErrorHandler} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {RouterModule}  from '@angular/router';
import {FlexLayoutModule} from '@angular/flex-layout';
import {HAMMER_GESTURE_CONFIG} from '@angular/platform-browser';
import {MATERIAL_COMPATIBILITY_MODE} from '@angular/material';

import {NhbkMaterialModule} from './nhbk-material.module';
import {AppComponent}  from './app.component';
import {routes} from './app.routes';
import {HttpModule} from '@angular/http';

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

@NgModule({
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        RouterModule.forRoot(routes),
        NhbkMaterialModule,
        FlexLayoutModule,
        HttpModule,
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
            provide: MATERIAL_COMPATIBILITY_MODE,
            useValue: true
        },
        {
            provide: ErrorHandler,
            useClass: NhbkErrorHandler
        },
        {
            provide: HAMMER_GESTURE_CONFIG,
            useClass: CustomHammerConfig
        }
    ]
})
export class AppModule {
}
