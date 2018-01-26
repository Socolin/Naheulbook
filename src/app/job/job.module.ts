import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {SharedModule} from '../shared/shared.module';

import {JobComponent, JobListComponent, JobSelectorComponent, JobService} from './';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        NhbkMaterialModule,
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
