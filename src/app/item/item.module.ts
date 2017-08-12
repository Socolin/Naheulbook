import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';

import {ActionModule} from '../action/action.module';
import {SharedModule} from '../shared/shared.module';
import {SkillModule} from '../skill/skill.module';
import {DateModule} from '../date/date.module';
import {EffectModule} from '../effect/effect.module';

import {
    AutocompleteSearchItemTemplateComponent,
    CreateItemTemplateComponent,
    EditItemComponent,
    ItemCategoryDirective,
    ItemListComponent,
    ItemTemplateComponent,
    ItemTemplateEditorComponent,
    ItemTemplateEditorModuleComponent,
    ItemTemplateService,
} from './';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        MaterialModule,
        SharedModule,
        SkillModule,
        DateModule,
        ActionModule,
        EffectModule,
    ],
    declarations: [
        CreateItemTemplateComponent,
        EditItemComponent,
        ItemCategoryDirective,
        ItemListComponent,
        ItemTemplateComponent,
        ItemTemplateEditorComponent,
        ItemTemplateEditorModuleComponent,
        AutocompleteSearchItemTemplateComponent,
    ],
    exports: [
        ItemListComponent,
        AutocompleteSearchItemTemplateComponent,
    ],
    providers: [
        ItemTemplateService,
    ]
})
export class ItemModule {
}
