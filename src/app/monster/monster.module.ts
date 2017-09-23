import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {ItemModule} from '../item/item.module';
import {SharedModule} from '../shared/shared.module';

import {
    MonsterColorSelectorComponent,
    MonsterEditorComponent,
    MonsterListComponent,
    MonsterTemplateComponent,
    MonsterTraitComponent,
    MonsterService,
    MonsterTemplateService
} from './';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        NhbkMaterialModule,
        ItemModule,
    ],
    declarations: [
        MonsterColorSelectorComponent,
        MonsterListComponent,
        MonsterTemplateComponent,
        MonsterEditorComponent,
        MonsterTraitComponent,
    ],
    providers: [
        MonsterService,
        MonsterTemplateService,
    ],
    exports: [
        MonsterColorSelectorComponent,
        MonsterListComponent,
        MonsterTemplateComponent,
    ],
})
export class MonsterModule {
}
