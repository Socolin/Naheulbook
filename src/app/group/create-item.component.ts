import {Component, Input, Output, EventEmitter, SimpleChanges, OnChanges} from '@angular/core';
import {Observable} from "rxjs";

import {ItemService} from "../item";
import {Character, Item} from "../character";
import {AutocompleteValue} from "../shared";
import {ItemTemplate} from "../item";
import {Loot} from "../loot";

@Component({
    selector: 'create-item',
    templateUrl: 'create-item.component.html',
})
export class CreateItemComponent implements OnChanges {
    @Input() character: Character;
    @Input() loot: Loot;
    @Output() onAddItem: EventEmitter<Item> = new EventEmitter<Item>();

    private mode: string = 'normal';
    private newItem: Item = new Item();
    private gemOption: any = {};
    private autocompleteItemCallback: Observable<AutocompleteValue[]> = this.updateAutocompleteItem.bind(this);

    constructor(private _itemService: ItemService) {
    }

    static getRandomInt(min: number, max: number): number {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }

    close() {
        this.onAddItem.emit(null);
        this.newItem = new Item();
        return false;
    }

    addItem() {
        if (this.mode === 'gem') {
            this.newItem.data.ug = this.gemOption['ug'];
        }
        this.onAddItem.emit(this.newItem);
        this.newItem = new Item();
        return false;
    }

    selectItemTemplate(itemTemplate: ItemTemplate, input) {
        this.newItem.template = itemTemplate;
        if (!this.newItem.data.notIdentified) {
            this.newItem.data.name = itemTemplate.name;
            input.focus();
        }
        this.newItem.data.description = itemTemplate.data.description;
        if (itemTemplate.data.quantifiable) {
            this.newItem.data.quantity = 1;
        }
    }

    updateAutocompleteItem(filter: string) {
        return this._itemService.searchItem(filter).map(
            list => list.map(e => new AutocompleteValue(e, e.name))
        );
    }

    setItemNotIdentified() {
        this.newItem.data.notIdentified = true;
    }

    setItemIdentified() {
        delete this.newItem.data.notIdentified;
    }

    setMode(m: string) {
        let oldItem = this.newItem;
        this.newItem = new Item();
        this.newItem.data.notIdentified = oldItem.data.notIdentified;
        this.mode = m;
        if (this.mode === 'gem') {
            let type = CreateItemComponent.getRandomInt(1, 2);
            if (type == 1) {
                this.setGemOption('type', 'raw');
            } else {
                this.setGemOption('type', 'cut');
            }
            this.setGemOption('ug', 1);
            this.setGemOption('number', CreateItemComponent.getRandomInt(1, 20));
        }
    }

    setGemRandomUg(dice: number) {
        this.setGemOption('ug', CreateItemComponent.getRandomInt(1, dice));
    }


    setGemOption(optName: string, value: any) {
        this.gemOption[optName] = value;
        this.updateGem();
        return false;
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ('gemOption' in changes) {
            this.updateGem();
        }
    }

    updateGem() {
        if (this.gemOption['type'] && this.gemOption['number']) {

            this._itemService.getGem(this.gemOption['type'], this.gemOption['number']).subscribe(
                itemTemplate => {
                    this.newItem.template = itemTemplate;
                    if (this.newItem.data.notIdentified) {
                        this.newItem.data.name = "Pierre précieuse";
                    } else {
                        this.newItem.data.name = itemTemplate.name;
                    }
                    this.newItem.data.description = itemTemplate.data.description;
                    this.newItem.data.quantity = 1;
                }
            );
        }
    }
}
