import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';

import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {MaterialWorkaroundModule} from '../material-workaround/material-workaround.module';

import {
    AutocompleteInputComponent,
    ConfirmGmModeDialogComponent,
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
import {ValueEditorSettingsDialogComponent} from './value-editor-settings-dialog.component';
import {CommonNavComponent} from './common-nav.component';


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
        ValueEditorSettingsDialogComponent,
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
    providers: [
        IconService,
        MiscService,
        NhbkDialogService,
    ]
})
export class SharedModule {
}
