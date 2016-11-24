import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {SharedModule} from '../shared/shared.module';

import {LoggedComponent, LoginComponent, UserProfileComponent, LoginService} from './';

@NgModule({
    imports: [
        CommonModule,
        SharedModule,
        FormsModule,
    ],
    declarations: [
        LoggedComponent,
        LoginComponent,
        UserProfileComponent,
    ],
    providers: [
        LoginService,
    ],
    exports: [
        LoggedComponent,
        LoginComponent,
        UserProfileComponent,
    ],
})
export class UserModule {
}
