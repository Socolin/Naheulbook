import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {
    AutocompleteInputComponent, ModifierPipe, ModifiersEditorComponent, PlusMinusPipe,
    StatRequirementsEditorComponent, TextareaAutosizeDirective, TextFormatterPipe, ValueEditorComponent
} from './';
import {IconSelectorComponent} from './icon-selector.component';
import {IconService} from './icon.service';
import {IconComponent} from './icon.component';
import {MiscService} from './misc.service';
import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';
import {NhbkDialogService} from './nhbk-dialog.service';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        MaterialModule,
        FlexLayoutModule,
    ],
    declarations: [
        AutocompleteInputComponent,
        ModifierPipe,
        ModifiersEditorComponent,
        PlusMinusPipe,
        StatRequirementsEditorComponent,
        TextareaAutosizeDirective,
        TextFormatterPipe,
        ValueEditorComponent,
        IconSelectorComponent,
        IconComponent,
    ],
    exports: [
        AutocompleteInputComponent,
        ModifierPipe,
        ModifiersEditorComponent,
        PlusMinusPipe,
        StatRequirementsEditorComponent,
        TextareaAutosizeDirective,
        TextFormatterPipe,
        ValueEditorComponent,
        IconSelectorComponent,
        IconComponent,
    ],
    providers: [
        IconService,
        MiscService,
        NhbkDialogService,
    ]
})
export class SharedModule {
}
