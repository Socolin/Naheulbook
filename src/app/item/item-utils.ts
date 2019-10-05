import {Item} from './item.model';
import {removeDiacritics} from '../shared';

export function isItemNameMatchingFilter(filter: string | undefined, item: Item) {
    if (!filter) {
        return true;
    }

    let cleanFilter = removeDiacritics(filter).toLowerCase();
    return removeDiacritics(item.data.name).toLowerCase().indexOf(cleanFilter) !== -1;
}


export function isChildItemMatchingFilter(filter: string | undefined, item: Item): boolean {
    if (isItemNameMatchingFilter(filter, item)) {
        return true;
    }

    if (!item.content) {
        return false;
    }

    for (let childItem of item.content) {
        if (isChildItemMatchingFilter(filter, childItem)) {
            return true;
        }
    }

    return false;
}


export type ItemSortType = 'none' | 'not_identified_first' | 'asc' | 'desc';

export function sortItemFn(sortType: ItemSortType, a: Item, b: Item) {
    switch (sortType) {
        case 'not_identified_first':
            if (a.data.notIdentified && b.data.notIdentified) {
                return 0;
            }
            if (a.data.notIdentified) {
                return -1;
            }
            if (b.data.notIdentified) {
                return 1;
            }

            if ((a.containerId || a.data.equiped) && (b.containerId || b.data.equiped)) {
                return a.data.name.localeCompare(b.data.name);
            }
            if (a.containerId || a.data.equiped) {
                return 1;
            }
            if (b.containerId || b.data.equiped) {
                return -1;
            }

            return a.data.name.localeCompare(b.data.name);
        case 'asc':
            return a.data.name.localeCompare(b.data.name);
        case 'desc':
            return b.data.name.localeCompare(a.data.name);
        default:
            return 0;
    }
}
