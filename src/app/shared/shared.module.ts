import {NgModule}      from '@angular/core';
import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";
import {
    AutocompleteInputComponent, ModifierPipe, ModifiersEditorComponent, PlusMinusPipe,
    StatRequirementsEditorComponent, TextareaAutosizeDirective, TextFormatterPipe, ValueEditorComponent
} from "./";
import {IconService} from "./icon.service";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
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
    ],
    providers: [
        IconService,
    ]
})
export class SharedModule {
}
