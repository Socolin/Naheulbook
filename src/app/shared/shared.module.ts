import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';

import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {MaterialWorkaroundModule} from '../material-workaround/material-workaround.module';

import {
    AutocompleteInputComponent,
    CommonNavComponent,
    ConfirmGmModeDialogComponent,
    GmModeService,
    IconComponent,
    IconSelectorComponent,
    IconService,
    MiscService,
    ModifierPipe,
    NhbkDialogService,
    PlusMinusPipe,
    PromptDialogComponent,
    StatRequirementsEditorComponent,
    TextareaAutosizeDirective,
    TextFormatterPipe,
    ValueEditorComponent,
} from './';


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
        RouterModule,
        MaterialWorkaroundModule,
    ],
    declarations: [
        AutocompleteInputComponent,
        ConfirmGmModeDialogComponent,
        CommonNavComponent,
        IconComponent,
        IconSelectorComponent,
        ModifierPipe,
        PlusMinusPipe,
        StatRequirementsEditorComponent,
        TextareaAutosizeDirective,
        TextFormatterPipe,
        ValueEditorComponent,
        PromptDialogComponent,
    ],
    exports: [
        MaterialWorkaroundModule,
        AutocompleteInputComponent,
        CommonNavComponent,
        IconComponent,
        IconSelectorComponent,
        ModifierPipe,
        PlusMinusPipe,
        StatRequirementsEditorComponent,
        TextareaAutosizeDirective,
        TextFormatterPipe,
        ValueEditorComponent,
    ],
    entryComponents: [
        ConfirmGmModeDialogComponent,
        IconSelectorComponent,
        PromptDialogComponent,
    ],
    providers: [
        IconService,
        MiscService,
        NhbkDialogService,
        GmModeService,
    ]
})
export class SharedModule {
}
