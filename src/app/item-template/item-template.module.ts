import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {ActionModule} from '../action/action.module';
import {SharedModule} from '../shared/shared.module';
import {SkillModule} from '../skill/skill.module';
import {DateModule} from '../date/date.module';
import {EffectModule} from '../effect/effect.module';

import {
    AutocompleteSearchItemTemplateComponent,
    CreateItemTemplateComponent,
    EditItemTemplateComponent,
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
        NhbkMaterialModule,
        SharedModule,
        SkillModule,
        DateModule,
        ActionModule,
        EffectModule,
    ],
    declarations: [
        CreateItemTemplateComponent,
        EditItemTemplateComponent,
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
export class ItemTemplateModule {
}
