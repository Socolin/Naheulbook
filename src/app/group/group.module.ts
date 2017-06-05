import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {
    CreateItemComponent, TargetSelectorComponent, GroupListComponent,
    GroupComponent, CreateGroupComponent, GroupService
} from './';

import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';

import {NotificationsModule} from '../notifications/notifications.module';
import {EventModule} from '../event/event.module';
import {DateModule} from '../date/date.module';
import {JobModule} from '../job/job.module';
import {OriginModule} from '../origin/origin.module';
import {ItemModule} from '../item/item.module';
import {CharacterModule} from '../character/character.module';
import {MonsterModule} from '../monster/monster.module';

import {FighterComponent} from './fighter.component';
import {GroupHistoryComponent} from './group-history.component';
import {SharedModule} from '../shared/shared.module';
import {FighterPanelComponent} from './fighter-panel.component';
import {GroupLootPanelComponent} from './group-loot-panel.component';
import {UsefullDataModule} from '../usefull-data/usefull-data.module';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        MaterialModule,
        FlexLayoutModule.forRoot(),
        SharedModule,
        CharacterModule,
        MonsterModule,
        NotificationsModule,
        DateModule,
        ItemModule,
        JobModule,
        OriginModule,
        EventModule,
        UsefullDataModule,
    ],
    declarations: [
        CreateItemComponent,
        GroupListComponent,
        GroupComponent,
        CreateGroupComponent,
        TargetSelectorComponent,
        FighterPanelComponent,
        GroupLootPanelComponent,
        GroupHistoryComponent,
        FighterComponent,
    ],
    providers: [GroupService]
})
export class GroupModule {
}
