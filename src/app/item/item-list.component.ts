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
    @Input()
    public actionIcon = 'delete';
    @Input()
    public actionName = 'Supprimer';
    @Output()
    public onAction: EventEmitter<Item> = new EventEmitter<Item>();
    @Output()
    public selectItem: EventEmitter<Item> = new EventEmitter<Item>();

    constructor() {
    }
}
