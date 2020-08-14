import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';

import {SharedModule} from '../shared/shared.module';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {LoggedComponent, LoginComponent, UserProfileComponent, LoginService} from './';
import {AuthGuard} from './auth-guard';
import {LogoutComponent} from './logout.component';
import { EnableShowInSearchComponent } from './enable-show-in-search.component';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        FormsModule,
        RouterModule,
        NhbkMaterialModule,
    ],
    declarations: [
        LoggedComponent,
        LoginComponent,
        LogoutComponent,
        UserProfileComponent,
        EnableShowInSearchComponent,
    ],
    providers: [
        LoginService,
        AuthGuard,
    ],
    exports: [
        LoggedComponent,
        LoginComponent,
        LogoutComponent,
        UserProfileComponent,
    ],
})
export class UserModule {
}
