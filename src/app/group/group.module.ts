import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {NotificationsModule} from '../notifications/notifications.module';
import {SharedModule} from '../shared/shared.module';
import {EventModule} from '../event/event.module';
import {DateModule} from '../date/date.module';
import {JobModule} from '../job/job.module';
import {OriginModule} from '../origin/origin.module';
import {ItemTemplateModule} from '../item-template/item-template.module';
import {CharacterModule} from '../character/character.module';
import {MonsterModule} from '../monster/monster.module';
import {UsefullDataModule} from '../usefull-data/usefull-data.module';
import {ItemModule} from '../item/item.module';

import {
    CreateGroupComponent,
    CreateItemComponent,
    FighterComponent,
    FighterIconComponent,
    FighterSelectorComponent,
    FighterPanelComponent,
    GroupComponent,
    GroupListComponent,
    GroupHistoryComponent,
    GroupLootPanelComponent,
    GroupService,
    TargetSelectorComponent,
} from './';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
        SharedModule,
        CharacterModule,
        MonsterModule,
        NotificationsModule,
        DateModule,
        ItemTemplateModule,
        JobModule,
        OriginModule,
        EventModule,
        UsefullDataModule,
        ItemModule
    ],
    declarations: [
        CreateGroupComponent,
        CreateItemComponent,
        FighterComponent,
        FighterIconComponent,
        FighterPanelComponent,
        FighterSelectorComponent,
        GroupComponent,
        GroupHistoryComponent,
        GroupListComponent,
        GroupLootPanelComponent,
        TargetSelectorComponent,
    ],
    entryComponents: [
        FighterSelectorComponent
    ],
    providers: [GroupService]
})
export class GroupModule {
}
