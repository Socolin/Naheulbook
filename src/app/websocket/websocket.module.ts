import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';

import {WebSocketService} from './websocket.service';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        MaterialModule,
        FlexLayoutModule,
    ],
    declarations: [
    ],
    exports: [
    ],
    providers: [
        WebSocketService,
    ]
})
export class WebsocketModule {
}
