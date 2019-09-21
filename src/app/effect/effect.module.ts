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
    EditEffectDialogComponent,
    EffectListComponent,
    EffectService,
    ModifierDetailComponent,
    ModifiersEditorComponent,
    StatModifierAdvancedDialogComponent,
    StatModifierEditorComponent,
} from './';
import { CreateEffectTypeDialogComponent } from './create-effect-type-dialog.component';

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
        ModifierDetailComponent,
        ModifiersEditorComponent,
        StatModifierAdvancedDialogComponent,
        StatModifierEditorComponent,
    ],
    providers: [
        EffectService
    ],
    entryComponents: [
        AddEffectDialogComponent,
        CreateEffectTypeDialogComponent,
        EditEffectDialogComponent,
        StatModifierAdvancedDialogComponent,
        AddStatModifierDialogComponent,
    ],
    exports: [
        ActiveEffectEditorComponent,
        AddEffectDialogComponent,
        EffectListComponent,
        ModifierDetailComponent,
        ModifiersEditorComponent,
        StatModifierEditorComponent,
    ],
})
export class EffectModule {
}
