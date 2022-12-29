import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {WebSocketService} from './websocket.service';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
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
