import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {
    MonsterColorSelectorComponent,
    MonsterListComponent,
    MonsterTemplateComponent,
    MonsterService,
    MonsterTemplateService
} from './';

import {SharedModule} from '../shared/shared.module';
import {MonsterEditorComponent} from './monster-editor.component';
import {MaterialModule} from '@angular/material';
import {MonsterTraitComponent} from './monster-trait.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        MaterialModule,
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
