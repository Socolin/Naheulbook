import {Injectable} from '@angular/core';
import {Observable, Observer} from 'rxjs';

import {Item, ItemData} from '../item';
import {ActiveStatsModifier} from '../shared';
import {IconDescription} from '../shared/icon.model';

type ActionName = 'change_icon'
    | 'delete'
    | 'edit_item_name'
    | 'equip'
    | 'give'
    | 'identify'
    | 'ignore_restrictions'
    | 'move_to_container'
    | 'read_skill_book'
    | 'unequip'
    | 'update_data'
    | 'update_modifiers'
    | 'update_quantity'
    | 'use_charge';

@Injectable()
export class ItemActionService {
    private actionObservers: { [actionName: string]: Observer<{ item: Item, data?: any }> } = {};

    public registerAction(actionName: 'change_icon'): Observable<{ item: Item, data: { icon: IconDescription } }>;
    public registerAction(actionName: 'delete'): Observable<{ item: Item }>;
    public registerAction(actionName: 'edit_item_name'): Observable<{ item: Item, data: {name: string, description?: string} }>;
    public registerAction(actionName: 'equip'): Observable<{ item: Item, data?: { level?: number } }>;
    public registerAction(actionName: 'give'): Observable<{ item: Item, data: { characterId: number, quantity?: number } }>;
    public registerAction(actionName: 'identify'): Observable<{ item: Item }>;
    public registerAction(actionName: 'ignore_restrictions'): Observable<{ item: Item, data?: boolean }>;
    public registerAction(actionName: 'move_to_container'): Observable<{ item: Item, data: {containerId?: number} }>;
    public registerAction(actionName: 'read_skill_book'): Observable<{ item: Item }>;
    public registerAction(actionName: 'unequip'): Observable<{ item: Item }>;
    public registerAction(actionName: 'update_data'): Observable<{ item: Item, data: ItemData }>;
    public registerAction(actionName: 'update_modifiers'): Observable<{ item: Item, data: ActiveStatsModifier[] }>;
    public registerAction(actionName: 'update_quantity'): Observable<{ item: Item, data: { quantity: number } }>;
    public registerAction(actionName: 'use_charge'): Observable<{ item: Item }>;
    public registerAction(actionName: ActionName): Observable<{ item: Item, data?: any }> {
        return new Observable(((observer) => {
            this.actionObservers[actionName] = observer;
        }));
    }

    public onAction(actionName: 'change_icon', item: Item, data: { icon: IconDescription });
    public onAction(actionName: 'delete', item: Item);
    public onAction(actionName: 'edit_item_name', item: Item, data: { name: string, description?: string });
    public onAction(actionName: 'equip', item: Item, data?: { level?: number });
    public onAction(actionName: 'give', item: Item, data: { characterId: number, quantity?: number });
    public onAction(actionName: 'identify', item: Item);
    public onAction(actionName: 'ignore_restrictions', item: Item, data?: boolean);
    public onAction(actionName: 'move_to_container', item: Item, data: {containerId?: number});
    public onAction(actionName: 'read_skill_book', item: Item);
    public onAction(actionName: 'unequip', item: Item);
    public onAction(actionName: 'update_data', item: Item, data: ItemData);
    public onAction(actionName: 'update_modifiers', item: Item, data: ActiveStatsModifier[]);
    public onAction(actionName: 'update_quantity', item: Item, data: { quantity: number });
    public onAction(actionName: 'use_charge', item: Item);
    public onAction(actionName: ActionName, item: Item, data?: any) {
        if (!(actionName in this.actionObservers)) {
            throw new Error('action: `' + actionName + '\' was not registered');
        }
        this.actionObservers[actionName].next({item: item, data: data});
        return false;
    }
}
