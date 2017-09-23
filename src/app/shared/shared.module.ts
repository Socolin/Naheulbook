import {NgModule}      from '@angular/core';
import {RouterModule} from '@angular/router';
import {FlexLayoutModule} from '@angular/flex-layout';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {
    AutocompleteInputComponent,
    CommonNavComponent,
    IconComponent,
    IconSelectorComponent,
    IconService,
    MiscService,
    ModifierPipe,
    NhbkDialogService,
    PlusMinusPipe,
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
        FlexLayoutModule,
        RouterModule,
    ],
    declarations: [
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
    exports: [
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
