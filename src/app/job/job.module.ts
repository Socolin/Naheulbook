import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';

import {SharedModule} from '../shared/shared.module';

import {JobComponent, JobListComponent, JobSelectorComponent, JobService} from './';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        MaterialModule,
        FlexLayoutModule,
    ],
    declarations: [
        JobComponent,
        JobListComponent,
        JobSelectorComponent,
    ],
    exports: [
        JobComponent,
        JobListComponent,
        JobSelectorComponent,
    ],
    providers: [
        JobService
    ],
})
export class JobModule {
}
