import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
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
        NhbkMaterialModule,
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
