import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {SharedModule} from '../shared/shared.module';
import {SkillModule} from '../skill/skill.module';
import {DateModule} from '../date/date.module';

import {ItemService} from './item.service';
import {ItemTemplateComponent} from './item-template.component';
import {
    CreateItemComponent,
    EditItemComponent,
    ItemListComponent,
    ItemTemplateEditorComponent,
    ItemTemplateEditorModuleComponent
} from './';
import {MaterialModule} from '@angular/material';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        MaterialModule.forRoot(),
        SharedModule,
        SkillModule,
        DateModule,
    ],
    declarations: [
        CreateItemComponent,
        EditItemComponent,
        ItemTemplateEditorComponent,
        ItemListComponent,
        ItemTemplateComponent,
        ItemTemplateEditorModuleComponent,
    ],
    exports: [
        ItemListComponent,
    ],
    providers: [
        ItemService,
    ]
})
export class ItemModule {
}
