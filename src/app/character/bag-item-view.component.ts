import {Input, Output, EventEmitter, OnInit, Component} from "@angular/core";
import {Item} from "./item.model";
import {Character} from "./character.model";

@Component({
    selector: 'bag-item-view',
    templateUrl: 'bag-item-view.component.html',
})
export class BagItemViewComponent implements OnInit {
    @Input() items: Item[];
    @Input() selectedItem: Item;
    @Input() character: Character;
    @Input() level: number = 0;
    @Input() end: number;
    @Input() gmView: boolean;
    @Input() ends: number[] = [];
    @Output() itemSelected: EventEmitter<Item> = new EventEmitter<Item>();

    selectItem(item) {
        this.itemSelected.emit(item);
        return false;
    }

    ngOnInit() {
        this.ends = JSON.parse(JSON.stringify(this.ends));
        if (this.end != null) {
            this.ends.push(this.end);
        }
    }
}
