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
    MonsterTraitComponent
} from './';
import { SelectMonsterTraitsDialogComponent } from './select-monster-traits-dialog.component';
import { MonsterTraitDialogComponent } from './monster-trait-dialog.component';
import { MonstersDialogComponent } from './monsters-dialog.component';

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
        MonsterTraitComponent,
        SelectMonsterTraitsDialogComponent,
        MonsterTraitDialogComponent,
        MonstersDialogComponent,
    ],
    entryComponents: [
        AddMonsterItemDialogComponent,
        EditMonsterTemplateDialogComponent,
        SelectMonsterTraitsDialogComponent,
        MonsterTraitDialogComponent,
        MonstersDialogComponent,
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
