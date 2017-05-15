import {NgModule, ErrorHandler} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {RouterModule}  from '@angular/router';
import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';

import {AppComponent}  from './app.component';
import {routes} from './app.routes';
import {HttpModule} from '@angular/http';

import {HomeModule} from './home/home.module';
import {CharacterModule} from './character/character.module';
import {EffectModule} from './effect/effect.module';
import {GroupModule} from './group/group.module';
import {ItemModule} from './item/item.module';
import {JobModule} from './job/job.module';
import {LocationModule} from './location/location.module';
import {MonsterModule} from './monster/monster.module';
import {NotificationsModule} from './notifications/notifications.module';
import {OriginModule} from './origin/origin.module';
import {QuestModule} from './quest/quest.module';
import {SharedModule} from './shared/shared.module';
import {SkillModule} from './skill/skill.module';
import {UserModule} from './user/user.module';

import {WebSocketService} from './shared/websocket.service';
import {DateModule} from './date/date.module';
import {NhbkErrorHandler} from './nhbk-error-handler';

import 'hammerjs';
import {PlayerHomeModule} from './home-player/home-player.module';

@NgModule({
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        RouterModule.forRoot(routes),
        MaterialModule,
        FlexLayoutModule.forRoot(),
        HttpModule,
        HomeModule,
        PlayerHomeModule,
        CharacterModule,
        EffectModule,
        GroupModule,
        ItemModule,
        JobModule,
        LocationModule,
        MonsterModule,
        NotificationsModule,
        OriginModule,
        QuestModule,
        SharedModule,
        SkillModule,
        UserModule,
        DateModule
    ],
    declarations: [
        AppComponent
    ],
    bootstrap: [
        AppComponent
    ],
    providers: [
        WebSocketService
        , {
            provide: ErrorHandler,
            useClass: NhbkErrorHandler
        }
    ]
})
export class AppModule {
}
