import {Input, Output, EventEmitter, OnInit, Component, OnChanges, SimpleChanges} from "@angular/core";
import {Item} from "./item.model";
import {Character} from "./character.model";

@Component({
    selector: 'bag-item-view',
    templateUrl: 'bag-item-view.component.html',
})
export class BagItemViewComponent implements OnInit, OnChanges {
    @Input() items: Item[];
    @Input() selectedItem: Item;
    @Input() character: Character;
    @Input() level: number = 0;
    @Input() iconMode: boolean;
    @Input() end: number;
    @Input() gmView: boolean;
    @Input() ends: number[] = [];
    @Output() itemSelected: EventEmitter<Item> = new EventEmitter<Item>();

    private itemsContainer: Item[] = [];
    private itemsContained: Item[] = [];

    selectItem(item) {
        this.itemSelected.emit(item);
        return false;
    }

    ngOnChanges(changes: SimpleChanges) {
        if ('items' in changes) {
            var container = [];
            var contained = [];

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
