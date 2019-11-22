import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {SharedModule} from '../shared/shared.module';
import {DateModule} from '../date/date.module';

import {
    ActiveEffectEditorComponent,
    AddEffectDialogComponent,
    AddStatModifierDialogComponent,
    CreateEffectTypeDialogComponent,
    EditEffectDialogComponent,
    EffectListComponent,
    EffectService,
    ModifierDetailsDialogComponent,
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
        ReactiveFormsModule,
    ],
    declarations: [
        ActiveEffectEditorComponent,
        AddEffectDialogComponent,
        AddStatModifierDialogComponent,
        CreateEffectTypeDialogComponent,
        EditEffectDialogComponent,
        EffectListComponent,
        ModifiersEditorComponent,
        StatModifierAdvancedDialogComponent,
        StatModifierEditorComponent,
        ModifierDetailsDialogComponent,
    ],
    providers: [
        EffectService
    ],
    entryComponents: [
        AddEffectDialogComponent,
        CreateEffectTypeDialogComponent,
        EditEffectDialogComponent,
        StatModifierAdvancedDialogComponent,
        ModifierDetailsDialogComponent,
        AddStatModifierDialogComponent,
    ],
    exports: [
        ActiveEffectEditorComponent,
        AddEffectDialogComponent,
        EffectListComponent,
        ModifiersEditorComponent,
        ModifierDetailsDialogComponent,
        StatModifierEditorComponent,
    ],
})
export class EffectModule {
}
