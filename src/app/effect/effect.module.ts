import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {FlexLayoutModule} from '@angular/flex-layout';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {SharedModule} from '../shared/shared.module';
import {DateModule} from '../date/date.module';

import {
    ActiveEffectEditorComponent,
    AddEffectModalComponent,
    CreateEffectComponent,
    EditEffectComponent,
    EffectEditorComponent,
    EffectListComponent,
    EffectService,
    ModifierDetailComponent,
    ModifiersEditorComponent,
    StatModifierEditorComponent,
} from './';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
        FlexLayoutModule,
        SharedModule,
        DateModule,
    ],
    declarations: [
        ActiveEffectEditorComponent,
        AddEffectModalComponent,
        CreateEffectComponent,
        EditEffectComponent,
        EffectEditorComponent,
        EffectListComponent,
        ModifierDetailComponent,
        ModifiersEditorComponent,
        StatModifierEditorComponent,
    ],
    providers: [
        EffectService
    ],
    exports: [
        ActiveEffectEditorComponent,
        AddEffectModalComponent,
        CreateEffectComponent,
        EditEffectComponent,
        EffectEditorComponent,
        EffectListComponent,
        ModifierDetailComponent,
        ModifiersEditorComponent,
        StatModifierEditorComponent,
    ],
})
export class EffectModule {
}
