import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';

import {SharedModule} from '../shared/shared.module';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {LoginService, UserService} from './';
import {AuthGuard} from './auth-guard';
import {LogoutComponent} from './logout.component';
import {EnableShowInSearchComponent} from './enable-show-in-search.component';
import {LoggedComponent} from './logged.component';
import {LoginComponent} from './login.component';
import {UserProfileComponent} from './user-profile.component';
import {ClipboardModule} from '@angular/cdk/clipboard';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        FormsModule,
        RouterModule,
        NhbkMaterialModule,
        ClipboardModule
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
        UserService,
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
