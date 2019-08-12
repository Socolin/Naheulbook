import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {SharedModule} from '../shared/shared.module';
import {DateModule} from '../date/date.module';

import {
    ActiveEffectEditorComponent,
    AddEffectDialogComponent,
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
        AddEffectDialogComponent,
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
        AddEffectDialogComponent,
        StatModifierAdvancedDialogComponent,
        AddStatModifierDialogComponent,
    ],
    exports: [
        ActiveEffectEditorComponent,
        AddEffectDialogComponent,
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
