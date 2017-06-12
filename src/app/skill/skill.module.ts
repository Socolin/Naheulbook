import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';

import {SharedModule} from '../shared/shared.module';

import {
    SkillComponent, SkillListComponent, SkillModifiersEditorComponent, SkillSelectorComponent,
    SkillService
} from './';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        MaterialModule,
        FlexLayoutModule,
        SharedModule,
    ],
    declarations: [
        SkillComponent,
        SkillListComponent,
        SkillModifiersEditorComponent,
        SkillSelectorComponent,
    ],
    providers: [
        SkillService
    ],
    exports: [
        SkillComponent,
        SkillListComponent,
        SkillModifiersEditorComponent,
        SkillSelectorComponent,
    ]
})
export class SkillModule {
}
