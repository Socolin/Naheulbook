import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {FlexLayoutModule} from '@angular/flex-layout';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {SharedModule} from '../shared/shared.module';

import {
    SkillComponent,
    SkillListComponent,
    SkillModifiersEditorComponent,
    SkillService,
} from './';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
        FlexLayoutModule,
        SharedModule,
    ],
    declarations: [
        SkillComponent,
        SkillListComponent,
        SkillModifiersEditorComponent,
    ],
    providers: [
        SkillService
    ],
    exports: [
        SkillComponent,
        SkillListComponent,
        SkillModifiersEditorComponent,
    ]
})
export class SkillModule {
}
