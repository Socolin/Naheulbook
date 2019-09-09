import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {SharedModule} from '../shared/shared.module';

import {
    ItemService,
} from './';
import {ItemListComponent} from './item-list.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
        SharedModule,
    ],
    declarations: [
        ItemListComponent
    ],
    providers: [
        ItemService
    ],
    exports: [
        ItemListComponent
    ]
})
export class ItemModule {
}
