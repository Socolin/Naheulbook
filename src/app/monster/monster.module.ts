import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {ItemTemplateModule} from '../item-template/item-template.module';
import {SharedModule} from '../shared/shared.module';

import {
    AddMonsterItemDialogComponent,
    EditMonsterTemplateDialogComponent,
    MonsterColorSelectorComponent,
    MonsterListComponent,
    MonsterService,
    MonsterTemplateComponent,
    MonsterTemplateService,
    MonsterTraitComponent,
    OldMonsterEditorComponent
} from './';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        NhbkMaterialModule,
        ItemTemplateModule,
        ReactiveFormsModule
    ],
    declarations: [
        AddMonsterItemDialogComponent,
        EditMonsterTemplateDialogComponent,
        MonsterColorSelectorComponent,
        MonsterListComponent,
        MonsterTemplateComponent,
        OldMonsterEditorComponent,
        MonsterTraitComponent,
    ],
    entryComponents: [
        AddMonsterItemDialogComponent,
        EditMonsterTemplateDialogComponent,
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
