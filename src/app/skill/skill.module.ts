import {NgModule}      from '@angular/core';
import {
    SkillComponent, SkillListComponent, SkillModifiersEditorComponent, SkillSelectorComponent,
    SkillService
} from "./";
import {CommonModule} from "@angular/common";
import {SharedModule} from "../shared/shared.module";
import {FormsModule} from "@angular/forms";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule
    ],
    declarations: [
        SkillComponent,
        SkillListComponent,
        SkillModifiersEditorComponent,
        SkillSelectorComponent,
    ],
    providers: [
        SkillService
    ],
    exports: [
        SkillComponent,
        SkillListComponent,
        SkillModifiersEditorComponent,
        SkillSelectorComponent,
    ]
})
export class SkillModule {
}
