import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';

import {ActionModule} from '../action/action.module';
import {SharedModule} from '../shared/shared.module';
import {SkillModule} from '../skill/skill.module';
import {DateModule} from '../date/date.module';

import {ItemService} from './item.service';

import {
    CreateItemComponent,
    EditItemComponent,
    ItemListComponent,
    ItemTemplateComponent,
    ItemTemplateEditorComponent,
    ItemTemplateEditorModuleComponent
} from './';

import {AutocompleteSearchItemTemplateComponent} from './autocomplete-search-item-template.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        MaterialModule,
        SharedModule,
        SkillModule,
        DateModule,
        ActionModule,
    ],
    declarations: [
        CreateItemComponent,
        EditItemComponent,
        ItemTemplateEditorComponent,
        ItemListComponent,
        ItemTemplateComponent,
        ItemTemplateEditorModuleComponent,
        AutocompleteSearchItemTemplateComponent,
    ],
    exports: [
        ItemListComponent,
        AutocompleteSearchItemTemplateComponent,
    ],
    providers: [
        ItemService,
    ]
})
export class ItemModule {
}
