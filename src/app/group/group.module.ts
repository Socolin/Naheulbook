import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

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
    AddLootDialogComponent,
    CreateGemDialogComponent,
    CreateGroupComponent,
    CreateItemComponent,
    CreateItemDialogComponent,
    FighterComponent,
    FighterIconComponent,
    FighterPanelComponent,
    FighterSelectorComponent,
    GroupComponent,
    GroupHistoryComponent,
    GroupListComponent,
    GroupLootPanelComponent,
    GroupService,
    TargetSelectorComponent,
} from './';
import { CharacterSheetDialogComponent } from './character-sheet-dialog.component';
import { EditMonsterDialogComponent } from './edit-monster-dialog.component';
import { ModifierDetailsDialogComponent } from './modifier-details-dialog.component';
import { MonsterInventoryDialogComponent } from './monster-inventory-dialog.component';

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
        ItemModule,
        ReactiveFormsModule
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
        CreateItemDialogComponent,
        AddLootDialogComponent,
        CreateGemDialogComponent,
        CharacterSheetDialogComponent,
        EditMonsterDialogComponent,
        ModifierDetailsDialogComponent,
        MonsterInventoryDialogComponent,
    ],
    entryComponents: [
        AddLootDialogComponent,
        CharacterSheetDialogComponent,
        CreateGemDialogComponent,
        CreateItemDialogComponent,
        EditMonsterDialogComponent,
        FighterSelectorComponent,
        ModifierDetailsDialogComponent,
        MonsterInventoryDialogComponent,
    ],
    providers: [GroupService]
})
export class GroupModule {
}
