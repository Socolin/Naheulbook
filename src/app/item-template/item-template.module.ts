import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {ActionModule} from '../action/action.module';
import {SharedModule} from '../shared/shared.module';
import {SkillModule} from '../skill/skill.module';
import {DateModule} from '../date/date.module';
import {EffectModule} from '../effect/effect.module';

import {
    AddItemTemplateEditorModuleDialogComponent,
    AutocompleteSearchItemTemplateComponent,
    CreateItemTemplateDialogComponent,
    EditItemTemplateDialogComponent,
    ItemTemplateComponent,
    ItemTemplateEditorComponent,
    ItemTemplateEditorModuleComponent,
    ItemTemplateListComponent,
    ItemTemplateService,
} from './';
import {ItemTemplateDialogComponent} from './item-template-dialog.component';
import {ItemTemplatesTableViewComponent} from './item-templates-table-view.component';
import { ItemTemplateDetailsComponent } from './item-template-details.component';
import { ItemTemplateDataProtectionPipe } from './item-template-data-protection.pipe';
import { ItemPricePipe } from './item-price.pipe';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
        ReactiveFormsModule,
        SharedModule,
        SkillModule,
        DateModule,
        ActionModule,
        EffectModule,
    ],
    declarations: [
        CreateItemTemplateDialogComponent,
        EditItemTemplateDialogComponent,
        ItemTemplateListComponent,
        ItemTemplateComponent,
        ItemTemplateEditorComponent,
        ItemTemplateEditorModuleComponent,
        AutocompleteSearchItemTemplateComponent,
        AddItemTemplateEditorModuleDialogComponent,
        ItemTemplateDialogComponent,
        ItemTemplatesTableViewComponent,
        ItemTemplateDetailsComponent,
        ItemTemplateDataProtectionPipe,
        ItemPricePipe,
    ],
    exports: [
        ItemTemplateListComponent,
        AutocompleteSearchItemTemplateComponent,
        ItemPricePipe,
    ],
    providers: [
        ItemTemplateService,
    ]
})
export class ItemTemplateModule {
}
