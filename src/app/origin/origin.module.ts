import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {SharedModule} from '../shared/shared.module';

import {
    OriginComponent,
    OriginListComponent,
    OriginSelectorComponent,
    OriginService
} from './';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
        SharedModule,
    ],
    declarations: [
        OriginComponent,
        OriginListComponent,
        OriginSelectorComponent,
    ],
    providers: [
        OriginService,
    ],
    exports: [
        OriginComponent,
        OriginListComponent,
        OriginSelectorComponent,
    ],
})
export class OriginModule {
}
