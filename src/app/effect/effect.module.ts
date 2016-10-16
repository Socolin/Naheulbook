import {NgModule}      from '@angular/core';
import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";

import {SharedModule} from "../shared/shared.module";

import {
    EffectEditorComponent, CreateEffectComponent, EffectListComponent, EditEffectComponent,
    EffectService
} from "./";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
    ],
    declarations: [
        EffectEditorComponent,
        CreateEffectComponent,
        EffectListComponent,
        EditEffectComponent,
    ],
    providers: [
        EffectService
    ],
    exports: [
        EffectEditorComponent,
        CreateEffectComponent,
        EffectListComponent,
        EditEffectComponent,
    ],
})
export class EffectModule {
}
