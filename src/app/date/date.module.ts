import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';

import {SharedModule} from '../shared/shared.module';
import {NotificationsModule} from '../notifications/notifications.module';

import {DateService} from './date.service';

import {DateComponent} from './date.component';

import {DurationSelectorDialogComponent} from './duration-selector-dialog.component';
import {NhbkDateDurationPipe, NhbkDateShortDurationPipe} from './nhbk-duration.pipe';
import {DurationSelectorComponent} from './duration-selector.component';
import { DateSelectorDialogComponent } from './date-selector-dialog.component';

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
        DurationSelectorDialogComponent,
        NhbkDateDurationPipe,
        NhbkDateShortDurationPipe,
        DurationSelectorComponent,
        DateSelectorDialogComponent,
    ],
    exports: [
        DateComponent,
        NhbkDateDurationPipe,
        NhbkDateShortDurationPipe,
        DurationSelectorComponent,
    ],
    entryComponents: [
        DateSelectorDialogComponent,
        DurationSelectorDialogComponent,
    ],
    providers: [DateService]
})
export class DateModule {
}
