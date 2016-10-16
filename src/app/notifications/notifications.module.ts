import {NgModule}      from '@angular/core';
import {SimpleNotificationsComponent} from "./";
import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
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
