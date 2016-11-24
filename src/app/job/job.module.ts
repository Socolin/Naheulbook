import {NgModule}      from '@angular/core';
import {JobComponent, JobListComponent, JobSelectorComponent, JobService} from './';
import {CommonModule} from '@angular/common';
import {SharedModule} from '../shared/shared.module';
import {FormsModule} from '@angular/forms';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule
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
