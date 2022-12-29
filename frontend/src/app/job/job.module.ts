import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {SharedModule} from '../shared/shared.module';

import {
    JobComponent,
    JobGmInfoComponent,
    JobListComponent,
    JobPlayerInfoComponent,
    JobSelectorComponent,
    JobService
} from './';

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
        JobPlayerInfoComponent,
        JobGmInfoComponent,
    ],
    exports: [
        JobComponent,
        JobListComponent,
        JobSelectorComponent,
        JobPlayerInfoComponent,
    ],
    providers: [
        JobService
    ],
})
export class JobModule {
}
