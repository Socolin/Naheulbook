import {Component, Input} from '@angular/core';
import {MatMenuPanel} from '@angular/material/menu';
import {Item} from '../item';

import {Character} from './character.model';
import {isChildItemMatchingFilter, ItemSortType, sortItemFn} from '../item/item-utils';

function sortItemContainerFirstFn(sortType: ItemSortType, a: Item, b: Item) {
    if (a.data.equiped) {
        return -1;
    }
    if (b.data.equiped) {
        return 1;
    }
    if (a.template.data.container) {
        return -1;
    }
    if (b.template.data.container) {
        return 1;
    }
    return sortItemFn(sortType, a, b);
}

@Component({
    selector: 'bag-item-view',
    templateUrl: './bag-item-view.component.html',
    styleUrls: ['./bag-item-view.component.scss'],
})
export class BagItemViewComponent {
    @Input() items: Item[];
    @Input() character: Character;
    @Input() gmView: boolean;
    @Input() itemFilterName?: string;
    @Input() sortType: ItemSortType;
    @Input() itemMenu: MatMenuPanel;
    public collapsed: {[itemId: number]: boolean} = {};

    get filteredBagItems(): Item[] {
        return this.items
            .filter(isChildItemMatchingFilter.bind(this, this.itemFilterName))
            .sort(sortItemContainerFirstFn.bind(this, this.sortType));
    }

    toggleExpand(item: Item) {
        this.collapsed[item.id] = !this.collapsed[item.id];
    }
}
