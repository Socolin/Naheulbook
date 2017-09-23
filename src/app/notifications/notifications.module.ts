import {NgModule}      from '@angular/core';
import {SimpleNotificationsComponent, NotificationsService} from './';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {NhbkMaterialModule} from '../nhbk-material.module';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
    ],
    providers: [
        NotificationsService,
    ],
    declarations: [
        SimpleNotificationsComponent,
    ],
    exports: [
        SimpleNotificationsComponent,
    ],
})
export class NotificationsModule {
}
