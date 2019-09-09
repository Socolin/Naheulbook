import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Item} from './item.model';

@Component({
    selector: 'app-item-list',
    templateUrl: './item-list.component.html',
    styleUrls: ['./item-list.component.scss']
})
export class ItemListComponent {
    @Input()
    public items: Item[];
    @Output()
    public deleteItem: EventEmitter<Item> = new EventEmitter<Item>();
    @Output()
    public selectItem: EventEmitter<Item> = new EventEmitter<Item>()

    constructor() {
    }
}
