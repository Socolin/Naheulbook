import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {FlexLayoutModule} from '@angular/flex-layout';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {WebSocketService} from './websocket.service';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
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
