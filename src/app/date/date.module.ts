import {NgModule}      from '@angular/core';
import {CommonModule} from "@angular/common";
import {SharedModule} from "../shared/shared.module";
import {FormsModule} from "@angular/forms";
import {NotificationsModule} from "../notifications/notifications.module";
import {DateSelectorComponent} from "./date-selector.component";
import {DateComponent} from "./date.component";
import {DateModifierComponent} from "./date-modifier.component";
import {DateService} from "./date.service";
import {NhbkDateDurationPipe} from "./nhbk-duration.pipe";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        NotificationsModule,
    ],
    declarations: [
        DateComponent,
        DateSelectorComponent,
        DateModifierComponent,
        NhbkDateDurationPipe,
    ],
    exports: [
        DateComponent,
        DateSelectorComponent,
        DateModifierComponent,
        NhbkDateDurationPipe,
    ],
    providers: [DateService]
})
export class DateModule {
}
