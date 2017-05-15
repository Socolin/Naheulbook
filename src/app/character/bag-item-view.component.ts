import {Input, Output, EventEmitter, OnInit, Component, OnChanges, SimpleChanges} from '@angular/core';
import {Item} from './item.model';
import {Character} from './character.model';

@Component({
    selector: 'bag-item-view',
    templateUrl: './bag-item-view.component.html',
    styleUrls: ['./bag-item-view.component.scss'],
})
export class BagItemViewComponent implements OnInit, OnChanges {
    @Input() items: Item[];
    @Input() selectedItem: Item;
    @Input() character: Character;
    @Input() level = 0;
    @Input() iconMode: boolean;
    @Input() end: number;
    @Input() gmView: boolean;
    @Input() ends: number[] = [];
    @Output() itemSelected: EventEmitter<Item> = new EventEmitter<Item>();

    public itemsContainer: Item[] = [];
    public itemsContained: Item[] = [];

    selectItem(item) {
        this.itemSelected.emit(item);
        return false;
    }

    ngOnChanges(changes: SimpleChanges) {
        if ('items' in changes) {
            let container = [];
            let contained = [];

            for (let i = 0; i < this.items.length; i++) {
                let item = this.items[i];
                if (item.content) {
                    container.push(item);
                }
                else {
                    contained.push(item);
                }
            }

            this.itemsContainer = container;
            this.itemsContained = contained;
        }
    }

    ngOnInit() {
        this.ends = JSON.parse(JSON.stringify(this.ends));
        if (this.end != null) {
            this.ends.push(this.end);
        }
    }
}
