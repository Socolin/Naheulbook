import {NgModule}      from '@angular/core';
import {RouterModule} from '@angular/router';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';

import {SharedModule} from '../shared/shared.module';
import {NotificationsModule} from '../notifications/notifications.module';
import {CharacterModule} from '../character/character.module';
import {UserModule} from '../user/user.module';
import {GroupModule} from '../group/group.module';

import {EffectModule} from '../effect/effect.module';
import {ItemModule} from '../item/item.module';
import {MonsterModule} from '../monster/monster.module';
import {OriginModule} from '../origin/origin.module';
import {QuestModule} from '../quest/quest.module';
import {SkillModule} from '../skill/skill.module';
import {DateModule} from '../date/date.module';
import {JobModule} from '../job/job.module';
import {LocationModule} from '../location/location.module';

import {routes} from './home-database.routes';
import {HomeDatabaseComponent} from './home-database.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        MaterialModule,
        FlexLayoutModule,
        RouterModule.forChild(routes),
        NotificationsModule,
        UserModule,
        CharacterModule,
        GroupModule,
        EffectModule,
        ItemModule,
        JobModule,
        LocationModule,
        MonsterModule,
        OriginModule,
        QuestModule,
        SkillModule,
        DateModule
    ],
    declarations: [
        HomeDatabaseComponent,
    ],
    exports: [
        HomeDatabaseComponent,
    ],
    providers: [
    ],
})
export class DatabaseHomeModule {
}
