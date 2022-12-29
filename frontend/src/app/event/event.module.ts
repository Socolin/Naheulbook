import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {SharedModule} from '../shared/shared.module';
import {DateModule} from '../date/date.module';

import {EventService} from './event.service';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {EventEditorComponent} from './event-editor.component';
import {EventsComponent} from './events.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
        SharedModule,
        DateModule
    ],
    declarations: [
        EventEditorComponent,
        EventsComponent
    ],
    providers: [
        EventService
    ],
    exports: [
        EventsComponent
    ],
})
export class EventModule {
}
