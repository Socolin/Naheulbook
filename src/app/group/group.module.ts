import {NgModule}      from '@angular/core';
import {
    CreateItemComponent, TargetSelectorComponent, MonsterEditableFieldComponent, GroupListComponent,
    GroupComponent, CreateGroupComponent, GroupService
} from "./";
import {CommonModule} from "@angular/common";
import {SharedModule} from "../shared/shared.module";
import {FormsModule} from "@angular/forms";
import {CharacterModule} from "../character/character.module";
import {MonsterModule} from "../monster/monster.module";
import {NotificationsModule} from "../notifications/notifications.module";
import {DateModule} from "../date/date.module";
import {FighterPanelComponent} from "./fighter-panel.component";
import {UsefullDataComponent} from "./usefull-data.component";
import {LootPanelComponent} from "./loot-panel.component";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        CharacterModule,
        MonsterModule,
        NotificationsModule,
        DateModule,
    ],
    declarations: [
        CreateItemComponent,
        GroupListComponent,
        GroupComponent,
        CreateGroupComponent,
        MonsterEditableFieldComponent,
        TargetSelectorComponent,
        FighterPanelComponent,
        UsefullDataComponent,
        LootPanelComponent,
    ],
    providers: [GroupService]
})
export class GroupModule {
}
