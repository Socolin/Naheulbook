import {Component, Input, EventEmitter, Output, ViewChild} from '@angular/core';
import {Observable} from 'rxjs';

import {AutocompleteInputComponent, AutocompleteValue} from '../shared/autocomplete-input.component';

import {ItemCategory, ItemTemplate} from './item-template.model';
import {ItemTemplateService} from './item-template.service';

@Component({
    selector: 'autocomplete-search-item-template',
    templateUrl: './autocomplete-search-item-template.component.html',
    styleUrls: ['./autocomplete-search-item-template.component.scss']
})
export class AutocompleteSearchItemTemplateComponent {
    @Input() clearOnSelect: boolean;
    @Input() placeholder = 'Chercher un objet';
    @Output() onSelect: EventEmitter<ItemTemplate> = new EventEmitter<ItemTemplate>();
    @ViewChild('autocomplete')
    autocomplete: AutocompleteInputComponent;

    public autocompleteItemCallback: Observable<AutocompleteValue[]> = this.updateAutocompleteItem.bind(this);

    constructor(private _itemTemplateService: ItemTemplateService) {
    }

    selectItemTemplate(itemTemplate: ItemTemplate) {
        this.onSelect.emit(itemTemplate);
    }

    focus() {
        this.autocomplete.focus();
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
                    let category = categoriesById[e.categoryId].section.name + ' - ' + categoriesById[e.categoryId].name;
                    let mdIcon;
                    if (e.source === 'community') {
                        mdIcon = 'group';
                    }
                    else if (e.source === 'private') {
                        mdIcon = 'lock';
                    }
                    return new AutocompleteValue(e, name, category, e.data.icon, mdIcon);
                });
            }
        );
    }
}
