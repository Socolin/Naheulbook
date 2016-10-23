import {NgModule}      from '@angular/core';
import {CreateItemComponent, EditItemComponent, ItemEditorComponent, ItemElementComponent, ItemListComponent} from "./";
import {CommonModule} from "@angular/common";
import {SharedModule} from "../shared/shared.module";
import {FormsModule} from "@angular/forms";
import {SkillModule} from "../skill/skill.module";
import {ItemService} from "./item.service";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        SkillModule
    ],
    declarations: [
        CreateItemComponent,
        EditItemComponent,
        ItemEditorComponent,
        ItemElementComponent,
        ItemListComponent,
    ],
    providers: [
        ItemService,
    ]
})
export class ItemModule {
}
