import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';

import {SharedModule} from '../shared/shared.module';
import {NotificationsModule} from '../notifications/notifications.module';

import {DateService} from './date.service';

import {DateComponent} from './date.component';

import {DateSelectorComponent} from './date-selector.component';
import {DateModifierComponent} from './date-modifier.component';
import {NhbkDateDurationPipe, NhbkDateShortDurationPipe} from './nhbk-duration.pipe';
import {DurationSelectorComponent} from './duration-selector.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        NotificationsModule,
        ReactiveFormsModule,
        NhbkMaterialModule,
    ],
    declarations: [
        DateComponent,
        DateSelectorComponent,
        DateModifierComponent,
        NhbkDateDurationPipe,
        NhbkDateShortDurationPipe,
        DurationSelectorComponent,
    ],
    exports: [
        DateComponent,
        DateSelectorComponent,
        DateModifierComponent,
        NhbkDateDurationPipe,
        NhbkDateShortDurationPipe,
        DurationSelectorComponent,
    ],
    providers: [DateService]
})
export class DateModule {
}
