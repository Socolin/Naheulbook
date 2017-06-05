import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';

import {SharedModule} from '../shared/shared.module';
import {DateModule} from '../date/date.module';

import {
    EffectEditorComponent, CreateEffectComponent, EffectListComponent, EditEffectComponent,
    EffectService, ModifierDetailComponent, StatModifierEditorComponent, ActiveEffectEditorComponent
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
        EffectEditorComponent,
        CreateEffectComponent,
        EffectListComponent,
        EditEffectComponent,
        ActiveEffectEditorComponent,
        StatModifierEditorComponent,
        ModifierDetailComponent,
    ],
    providers: [
        EffectService
    ],
    exports: [
        EffectEditorComponent,
        CreateEffectComponent,
        EffectListComponent,
        EditEffectComponent,
        ActiveEffectEditorComponent,
        StatModifierEditorComponent,
        ModifierDetailComponent,
    ],
})
export class EffectModule {
}
