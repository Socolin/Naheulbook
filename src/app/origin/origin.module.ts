import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';

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
        MaterialModule,
        FlexLayoutModule,
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
