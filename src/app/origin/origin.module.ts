import {NgModule}      from '@angular/core';
import {OriginComponent, OriginListComponent, OriginSelectorComponent, OriginService} from './';
import {CommonModule} from '@angular/common';
import {SharedModule} from '../shared/shared.module';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        MaterialModule,
        FlexLayoutModule.forRoot(),
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
