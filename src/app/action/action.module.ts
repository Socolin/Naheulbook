import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {SharedModule} from '../shared/shared.module';
import {NhbkActionComponent} from './nhbk-action.component';
import {NhbkActionEditorDialogComponent} from './nhbk-action-editor-dialog.component';
import {EffectModule} from '../effect/effect.module';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
        SharedModule,
        EffectModule,
    ],
    declarations: [
        NhbkActionComponent,
        NhbkActionEditorDialogComponent,
    ],
    exports: [
        NhbkActionComponent,
        NhbkActionEditorDialogComponent,
    ],
    providers: []
})
export class ActionModule {
}
