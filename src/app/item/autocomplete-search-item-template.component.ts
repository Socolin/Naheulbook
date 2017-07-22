import {Component, Input, EventEmitter, Output} from '@angular/core';
import {Observable} from 'rxjs';

import {AutocompleteValue} from '../shared/autocomplete-input.component';

import {
    ItemCategory,
    ItemTemplate,
    ItemTemplateService
} from './';

@Component({
    selector: 'autocomplete-search-item-template',
    templateUrl: './autocomplete-search-item-template.component.html',
    styleUrls: ['./autocomplete-search-item-template.component.scss']
})
export class AutocompleteSearchItemTemplateComponent {
    @Input() clearOnSelect: boolean;
    @Input() placeholder = 'Chercher un objet';
    @Output() onSelect: EventEmitter<ItemTemplate> = new EventEmitter<ItemTemplate>();

    public autocompleteItemCallback: Observable<AutocompleteValue[]> = this.updateAutocompleteItem.bind(this);

    constructor(private _itemTemplateService: ItemTemplateService) {
    }

    selectItemTemplate(itemTemplate: ItemTemplate) {
        this.onSelect.emit(itemTemplate);
    }

    updateAutocompleteItem(filter: string) {
        return Observable.forkJoin(
            this._itemTemplateService.getCategoriesById(),
            this._itemTemplateService.searchItem(filter)
        ).map(
            ([categoriesById, list]: [{ [categoryId: number]: ItemCategory }, ItemTemplate[]]) => {
                return list.map(e => {
                    let name = e.name;
                    if (e.data.enchantment !== undefined) {
                        name += ' (Ench. ' + e.data.enchantment + ')';
                    }
                    let category = categoriesById[e.category].type.name + ' - ' + categoriesById[e.category].name;
                    return new AutocompleteValue(e, name, category, e.data.icon);
                });
            }
        );
    }
}
