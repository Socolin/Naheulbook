import {NgModule}      from '@angular/core';
import {SimpleNotificationsComponent, NotificationsService} from "./";
import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
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
