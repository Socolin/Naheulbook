import {forkJoin, Observable} from 'rxjs';

import {map} from 'rxjs/operators';
import {Component, EventEmitter, Input, Output, ViewChild} from '@angular/core';

import {AutocompleteInputComponent, AutocompleteValue} from '../shared';

import {ItemTemplate, ItemTemplateSubCategory} from './item-template.model';
import {ItemTemplateService} from './item-template.service';
import { AutocompleteInputComponent as AutocompleteInputComponent_1 } from '../shared/autocomplete-input.component';

@Component({
    selector: 'autocomplete-search-item-template',
    templateUrl: './autocomplete-search-item-template.component.html',
    styleUrls: ['./autocomplete-search-item-template.component.scss'],
    imports: [AutocompleteInputComponent_1]
})
export class AutocompleteSearchItemTemplateComponent {
    @Input() clearOnSelect: boolean;
    @Input() placeholder = 'Chercher un objet';
    @Output() selected: EventEmitter<ItemTemplate> = new EventEmitter<ItemTemplate>();
    @ViewChild('autocomplete', {static: true})
    autocomplete: AutocompleteInputComponent;

    public autocompleteItemCallback:  (filter: string) => Observable<AutocompleteValue[]> = this.updateAutocompleteItem.bind(this);

    constructor(
        private readonly itemTemplateService: ItemTemplateService,
    ) {
    }

    selectItemTemplate(itemTemplate: ItemTemplate) {
        this.selected.emit(itemTemplate);
    }

    updateAutocompleteItem(filter: string) {
        return forkJoin([
            this.itemTemplateService.getSubCategoriesById(),
            this.itemTemplateService.searchItem(filter)
        ]).pipe(map(
            ([subCategoriesById, list]: [{ [subCategoryId: number]: ItemTemplateSubCategory }, ItemTemplate[]]) => {
                return list.map(e => {
                    let name = e.name;
                    if (e.data.enchantment !== undefined) {
                        name += ' (Ench. ' + e.data.enchantment + ')';
                    }
                    let categoryString = subCategoriesById[e.subCategoryId].section.name + ' - ' + subCategoriesById[e.subCategoryId].name;
                    let mdIcon;
                    if (e.source === 'community') {
                        mdIcon = 'group';
                    } else if (e.source === 'private') {
                        mdIcon = 'lock';
                    }
                    return new AutocompleteValue(e, name, categoryString, e.data.icon, mdIcon);
                });
            }
        ));
    }
}
