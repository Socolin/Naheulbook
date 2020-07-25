import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {SharedModule} from '../shared/shared.module';

import {
    ItemService,
} from './';
import {ItemListComponent} from './item-list.component';
import { ItemDialogComponent } from './item-dialog.component';
import {DateModule} from '../date/date.module';
import {ActionModule} from '../action/action.module';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
        SharedModule,
        DateModule,
        ActionModule,
    ],
    declarations: [
        ItemListComponent,
        ItemDialogComponent
    ],
    providers: [
        ItemService
    ],
    exports: [
        ItemDialogComponent,
        ItemListComponent
    ]
})
export class ItemModule {
}
