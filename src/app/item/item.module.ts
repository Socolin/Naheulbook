import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {SharedModule} from '../shared/shared.module';
import {SkillModule} from '../skill/skill.module';
import {DateModule} from '../date/date.module';

import {ItemService} from './item.service';
import {ItemTemplateComponent} from './item-template.component';
import {CreateItemComponent, EditItemComponent, ItemEditorComponent, ItemListComponent} from './';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        SkillModule,
        DateModule,
    ],
    declarations: [
        CreateItemComponent,
        EditItemComponent,
        ItemEditorComponent,
        ItemListComponent,
        ItemTemplateComponent,
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
