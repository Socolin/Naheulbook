import {forkJoin, Observable} from 'rxjs';

import {map} from 'rxjs/operators';
import {Component, EventEmitter, Input, Output, ViewChild} from '@angular/core';

import {AutocompleteInputComponent, AutocompleteValue} from '../shared';

import {ItemTemplate, ItemTemplateCategory} from './item-template.model';
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
    @ViewChild('autocomplete', {static: true})
    autocomplete: AutocompleteInputComponent;

    public autocompleteItemCallback: Observable<AutocompleteValue[]> = this.updateAutocompleteItem.bind(this);

    constructor(
        private readonly itemTemplateService: ItemTemplateService,
    ) {
    }

    selectItemTemplate(itemTemplate: ItemTemplate) {
        this.onSelect.emit(itemTemplate);
    }

    updateAutocompleteItem(filter: string) {
        return forkJoin([
            this.itemTemplateService.getCategoriesById(),
            this.itemTemplateService.searchItem(filter)
        ]).pipe(map(
            ([categoriesById, list]: [{ [categoryId: number]: ItemTemplateCategory }, ItemTemplate[]]) => {
                return list.map(e => {
                    let name = e.name;
                    if (e.data.enchantment !== undefined) {
                        name += ' (Ench. ' + e.data.enchantment + ')';
                    }
                    let category = categoriesById[e.categoryId].section.name + ' - ' + categoriesById[e.categoryId].name;
                    let mdIcon;
                    if (e.source === 'community') {
                        mdIcon = 'group';
                    } else if (e.source === 'private') {
                        mdIcon = 'lock';
                    }
                    return new AutocompleteValue(e, name, category, e.data.icon, mdIcon);
                });
            }
        ));
    }
}
