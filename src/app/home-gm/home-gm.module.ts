import {NgModule}      from '@angular/core';
import {RouterModule} from '@angular/router';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms'
import {FlexLayoutModule} from '@angular/flex-layout';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {SharedModule} from '../shared/shared.module';
import {NotificationsModule} from '../notifications/notifications.module';
import {CharacterModule} from '../character/character.module';
import {UserModule} from '../user/user.module';
import {GroupModule} from '../group/group.module';


import {HomeGmComponent} from './home-gm.component';
import {EffectModule} from '../effect/effect.module';
import {ItemTemplateModule} from '../item-template/item-template.module';
import {MonsterModule} from '../monster/monster.module';
import {OriginModule} from '../origin/origin.module';
import {QuestModule} from '../quest/quest.module';
import {SkillModule} from '../skill/skill.module';
import {DateModule} from '../date/date.module';
import {JobModule} from '../job/job.module';
import {LocationModule} from '../location/location.module';

import {routes} from './home-gm.routes';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        NhbkMaterialModule,
        FlexLayoutModule,
        RouterModule.forChild(routes),
        NotificationsModule,
        UserModule,
        CharacterModule,
        GroupModule,
        EffectModule,
        ItemTemplateModule,
        JobModule,
        LocationModule,
        MonsterModule,
        OriginModule,
        QuestModule,
        SkillModule,
        DateModule
    ],
    declarations: [
        HomeGmComponent,
    ],
    exports: [
        HomeGmComponent,
    ],
    providers: [
    ],
})
export class GmHomeModule {
}
