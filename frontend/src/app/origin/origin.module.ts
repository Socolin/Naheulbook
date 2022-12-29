import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {SharedModule} from '../shared/shared.module';

import {
    OriginComponent,
    OriginGmInfoComponent,
    OriginListComponent,
    OriginPlayerInfoComponent,
    OriginSelectorComponent,
    OriginService
} from './';
import { NameGeneratorDialogComponent } from './name-generator-dialog.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        NhbkMaterialModule,
        SharedModule,
    ],
    declarations: [
        OriginComponent,
        OriginListComponent,
        OriginSelectorComponent,
        OriginPlayerInfoComponent,
        OriginGmInfoComponent,
        NameGeneratorDialogComponent,
    ],
    providers: [
        OriginService,
    ],
    exports: [
        OriginComponent,
        OriginListComponent,
        OriginSelectorComponent,
        OriginPlayerInfoComponent,
    ],
})
export class OriginModule {
}
