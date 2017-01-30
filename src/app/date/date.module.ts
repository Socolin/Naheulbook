import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';

import {SharedModule} from '../shared/shared.module';
import {NotificationsModule} from '../notifications/notifications.module';

import {DateService} from './date.service';

import {DateComponent} from './date.component';

import {DateSelectorComponent} from './date-selector.component';
import {DateModifierComponent} from './date-modifier.component';
import {NhbkDateDurationPipe} from './nhbk-duration.pipe';
import {DurationSelectorComponent} from './duration-selector.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        NotificationsModule,
        MaterialModule.forRoot(),
        FlexLayoutModule.forRoot(),
    ],
    declarations: [
        DateComponent,
        DateSelectorComponent,
        DateModifierComponent,
        NhbkDateDurationPipe,
        DurationSelectorComponent,
    ],
    exports: [
        DateComponent,
        DateSelectorComponent,
        DateModifierComponent,
        NhbkDateDurationPipe,
        DurationSelectorComponent,
    ],
    providers: [DateService]
})
export class DateModule {
}
