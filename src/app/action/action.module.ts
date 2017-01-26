import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';

import {SharedModule} from '../shared/shared.module';
import {NhbkActionComponent} from './nhbk-action.component';
import {NhbkActionEditorComponent} from './nhbk-action-editor.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        MaterialModule.forRoot(),
        SharedModule,
    ],
    declarations: [
        NhbkActionComponent,
        NhbkActionEditorComponent,
    ],
    exports: [
        NhbkActionComponent,
        NhbkActionEditorComponent,
    ],
    providers: []
})
export class ActionModule {
}
