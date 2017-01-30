import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from "@angular/material";
import {FlexLayoutModule} from "@angular/flex-layout";

import {SharedModule} from '../shared/shared.module';
import {DateModule} from '../date/date.module';

import {
    EffectEditorComponent, CreateEffectComponent, EffectListComponent, EditEffectComponent,
    EffectService
} from "./";
import {CharacterEffectEditorComponent} from './character-effect-editor.component';
import {CharacterModifierEditorComponent} from './stats-modifier-editor.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        MaterialModule.forRoot(),
        FlexLayoutModule.forRoot(),
        SharedModule,
        DateModule,
    ],
    declarations: [
        EffectEditorComponent,
        CreateEffectComponent,
        EffectListComponent,
        EditEffectComponent,
        CharacterEffectEditorComponent,
        CharacterModifierEditorComponent,
    ],
    providers: [
        EffectService
    ],
    exports: [
        EffectEditorComponent,
        CreateEffectComponent,
        EffectListComponent,
        EditEffectComponent,
        CharacterEffectEditorComponent,
        CharacterModifierEditorComponent,
    ],
})
export class EffectModule {
}
