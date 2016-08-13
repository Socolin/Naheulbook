import {Component, Input, Output, EventEmitter} from '@angular/core';
import {Observable} from "rxjs";

import {ItemService} from "../item/item.service";
import {Character} from "../character/character.model";
import {Item} from "../character/item.model";
import {AutocompleteInputComponent, AutocompleteValue} from "../shared/autocomplete-input.component";
import {ItemTemplate} from "../item/item-template.model";

@Component({
    moduleId: module.id,
    selector: 'create-item',
    templateUrl: 'create-item.component.html',
    directives: [AutocompleteInputComponent]
})
export class CreateItemComponent {
    @Input() character: Character;
    @Output() onAddItem: EventEmitter<Item> = new EventEmitter<Item>();

    private newItem: Item = new Item();
    private autocompleteItemCallback: Observable<AutocompleteValue[]> = this.updateAutocompleteItem.bind(this);

    constructor(private _itemService: ItemService) {
    }

    close() {
        this.onAddItem.emit(null);
        this.newItem = new Item();
        return false;
    }

    addItem() {
        this._itemService.addItem(this.character.id, this.newItem.template.id, this.newItem.data).subscribe();
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
}
