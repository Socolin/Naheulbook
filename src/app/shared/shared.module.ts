import {NgModule}      from '@angular/core';
import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";
import {
    AutocompleteInputComponent, ModifierPipe, ModifiersEditorComponent, PlusMinusPipe,
    StatRequirementsEditorComponent, TextareaAutosizeDirective, TextFormatterPipe, ValueEditorComponent
} from "./";
import {IconSelectorComponent} from "./icon-selector.component";
import {IconService} from "./icon.service";
import {IconComponent} from "./icon.component";
import {JsonService} from "./json-service";
import {MiscService} from "./misc.service";

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
    ]
})
export class SharedModule {
}
