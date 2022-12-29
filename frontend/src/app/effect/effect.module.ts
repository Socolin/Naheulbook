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
import {MarkdownModule} from '../markdown/markdown.module';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
        SharedModule,
        DateModule,
        ReactiveFormsModule,
        MarkdownModule,
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
