import {Component, Input, EventEmitter, Output} from '@angular/core';
import {Observable} from 'rxjs';

import {AutocompleteValue} from '../shared/autocomplete-input.component';
import {ItemService} from './item.service';
import {ItemTemplate} from './item-template.model';

@Component({
    selector: 'autocomplete-search-item-template',
    templateUrl: './autocomplete-search-item-template.component.html',
    styleUrls: ['./autocomplete-search-item-template.component.scss']
})
export class AutocompleteSearchItemTemplateComponent {
    @Input() clearOnSelect: boolean;
    @Output() onSelect: EventEmitter<ItemTemplate> = new EventEmitter<ItemTemplate>();

    public autocompleteItemCallback: Observable<AutocompleteValue[]> = this.updateAutocompleteItem.bind(this);

    constructor(private _itemService: ItemService) {
    }

    selectItemTemplate(itemTemplate: ItemTemplate) {
        this.onSelect.emit(itemTemplate);
    }

    updateAutocompleteItem(filter: string) {
        return this._itemService.searchItem(filter).map(
            list => list.map(e => new AutocompleteValue(e, e.name))
        );
    }
}
