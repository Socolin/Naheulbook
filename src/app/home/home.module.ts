import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';

import {SharedModule} from '../shared/shared.module';

import {HomeService} from './home.service';
import {HomeComponent} from './home.component';
import {CreditComponent} from './credit.component';
import {RouterModule} from '@angular/router';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        MaterialModule,
        FlexLayoutModule,
        RouterModule,
    ],
    declarations: [
        HomeComponent,
        CreditComponent,
    ],
    exports: [
        HomeComponent,
        CreditComponent,
    ],
    providers: [
        HomeService,
    ],
})
export class HomeModule {
}
