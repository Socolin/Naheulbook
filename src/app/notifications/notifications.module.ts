import {NgModule}      from '@angular/core';
import {SimpleNotificationsComponent, NotificationsService} from './';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        MaterialModule,
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
