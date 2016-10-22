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

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        CharacterModule,
        MonsterModule,
        NotificationsModule,
    ],
    declarations: [
        CreateItemComponent,
        GroupListComponent,
        GroupComponent,
        CreateGroupComponent,
        MonsterEditableFieldComponent,
        TargetSelectorComponent,
    ],
    providers: [GroupService]
})
export class GroupModule {
}
