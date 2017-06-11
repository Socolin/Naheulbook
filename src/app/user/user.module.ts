import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {SharedModule} from '../shared/shared.module';

import {LoggedComponent, LoginComponent, UserProfileComponent, LoginService} from './';
import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';
import {AuthGuard} from './auth-guard';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        FormsModule,
        MaterialModule,
        FlexLayoutModule
    ],
    declarations: [
        LoggedComponent,
        LoginComponent,
        UserProfileComponent,
    ],
    providers: [
        LoginService,
        AuthGuard,
    ],
    exports: [
        LoggedComponent,
        LoginComponent,
        UserProfileComponent,
    ],
})
export class UserModule {
}
