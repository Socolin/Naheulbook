import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {SharedModule} from '../shared/shared.module';
import {DateModule} from '../date/date.module';

import {
    ActiveEffectEditorComponent,
    AddEffectModalComponent,
    AddStatModifierDialogComponent,
    CreateEffectComponent,
    EditEffectComponent,
    EffectEditorComponent,
    EffectListComponent,
    EffectService,
    ModifierDetailComponent,
    ModifiersEditorComponent,
    StatModifierAdvancedDialogComponent,
    StatModifierEditorComponent,
} from './';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
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
        StatModifierAdvancedDialogComponent,
        AddStatModifierDialogComponent,
    ],
    providers: [
        EffectService
    ],
    entryComponents: [
        StatModifierAdvancedDialogComponent,
        AddStatModifierDialogComponent,
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
