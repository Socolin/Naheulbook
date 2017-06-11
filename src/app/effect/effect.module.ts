import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';

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
    StatModifierEditorComponent,
} from './';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        MaterialModule,
        FlexLayoutModule.forRoot(),
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
        StatModifierEditorComponent,
    ],
})
export class EffectModule {
}
