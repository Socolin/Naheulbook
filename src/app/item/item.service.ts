import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';

import {ItemTemplate, ItemSection, ItemSlot} from '../item';
import {Item} from "../character";

@Injectable()
export class ItemService {
    private itemBySection: {[sectionId: number]: ReplaySubject<ItemTemplate[]>} = {};
    private itemSections: ReplaySubject<ItemSection[]>;
    private slots: ReplaySubject<ItemSlot[]>;

    constructor(private _http: Http) {
    }

    getSectionsList(): Observable<ItemSection[]> {
        if (!this.itemSections || this.itemSections.isUnsubscribed) {
            this.itemSections = new ReplaySubject<ItemSection[]>(1);

            this._http.get('/api/item/category/list')
                .map(res => res.json())
                .subscribe(
                    categoryList => {
                        this.itemSections.next(categoryList);
                    },
                    error => {
                        this.itemSections.error(error);
                    }
                );
        }
        return this.itemSections;
    }

    getItems(section: ItemSection): Observable<ItemTemplate[]> {
        if (!(section.id in this.itemBySection) || this.itemBySection[section.id].isUnsubscribed) {
            this.itemBySection[section.id] = new ReplaySubject<ItemTemplate[]>(1);
            this._http.post('/api/item/list', JSON.stringify({
                typeId: section.id
            })).map(res => res.json()).subscribe(
                effects => {
                    this.itemBySection[section.id].next(effects);
                },
                error => {
                    this.itemBySection[section.id].error(error);
                }
            );
        }
        return this.itemBySection[section.id];
    }

    create(item): Observable<ItemTemplate> {
        return this._http.post('/api/item/create', JSON.stringify({item: item}))
            .map(item => item.json());
    }

    getItem(id): Observable<ItemTemplate> {
        return this._http.post('/api/item/detail', JSON.stringify({id: id}))
            .map(res => res.json());
    }

    searchItem(filter): Observable<ItemTemplate[]> {
        return this._http.post('/api/item/search', JSON.stringify({filter: filter}))
            .map(res => res.json());
    }

    getSlots(): Observable<ItemSlot[]> {
        if (!this.slots || this.slots.isUnsubscribed) {
            this.slots = new ReplaySubject<ItemSlot[]>(1);

            this._http.get('/api/item/slot/list')
                .map(res => res.json())
                .subscribe(
                    categoryList => {
                        this.slots.next(categoryList);
                    },
                    error => {
                        this.slots.error(error);
                    }
                );
        }
        return this.slots;
    }

    editItemTemplate(item): Observable<ItemTemplate> {
        return this._http.post('/api/item/edit', JSON.stringify({item: item}))
            .map(res => res.json());
    }

    deleteItem(itemId: number): Observable<Item> {
        return this._http.post('/api/item/delete', JSON.stringify({itemId: itemId}))
            .map(res => res.json());
    }

    addItem(characterId: number, itemTemplateId: number, itemCustomName: string, itemCustomDescription: string, itemQuantity): Observable<Item> {
        return this._http.post('/api/item/add', JSON.stringify({
            itemTemplateId: itemTemplateId,
            characterId: characterId,
            itemCustomName: itemCustomName,
            itemQuantity: itemQuantity,
            itemCustomDescription: itemCustomDescription
        })).map(res => res.json());
    }

    equipItem(itemId: number, level: number): Observable<Item> {
        return this._http.post('/api/item/equip', JSON.stringify({
            itemId: itemId,
            level: level
        })).map(res => res.json());
    }

    moveToContainer(itemId: number, containerId: number): Observable<Item> {
        return this._http.post('/api/item/moveToContainer', JSON.stringify({
            itemId: itemId,
            containerId: containerId
        })).map(res => res.json());
    }

    updateQuantity(itemId: number, quantity): Observable<Item> {
        return this._http.post('/api/item/updateQuantity', JSON.stringify({
            itemId: itemId,
            quantity: quantity
        })).map(res => res.json());
    }

    readBook(itemId: number): Observable<Item> {
        return this._http.post('/api/item/readBook', JSON.stringify({
            itemId: itemId,
        })).map(res => res.json());
    }

    updateCharge(itemId: number, charge): Observable<Item> {
        return this._http.post('/api/item/updateCharge', JSON.stringify({
            itemId: itemId,
            charge: charge
        })).map(res => res.json());
    }

    updateItem(itemId: number, itemData): Observable<Item> {
        return this._http.post('/api/item/updateItem', JSON.stringify({
            itemId: itemId,
            itemData: itemData
        })).map(res => res.json());
    }
}
