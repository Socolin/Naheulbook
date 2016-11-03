import {NgModule}      from '@angular/core';
import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";

import {MonsterColorSelectorComponent, MonsterListComponent, MonsterTemplateComponent, MonsterService} from "./";

import {SharedModule} from "../shared/shared.module";
import {MonsterEditorComponent} from "./monster-editor.component";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
    ],
    declarations: [
        MonsterColorSelectorComponent,
        MonsterListComponent,
        MonsterTemplateComponent,
        MonsterEditorComponent,
    ],
    providers: [
        MonsterService,
    ],
    exports: [
        MonsterColorSelectorComponent,
        MonsterListComponent,
        MonsterTemplateComponent,
    ],
})
export class MonsterModule {
}
