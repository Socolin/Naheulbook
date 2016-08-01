import {Injectable, EventEmitter} from '@angular/core';
import {Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';

import {ItemTemplate, ItemSection, ItemSlot} from '../item';
import {Item} from "../character";
import {JsonService} from '../shared/json-service';
import {ItemCategory} from './item-template.model';

@Injectable()
export class ItemService extends JsonService {
    private itemBySection: {[sectionId: number]: ReplaySubject<ItemTemplate[]>} = {};
    private itemSections: ReplaySubject<ItemSection[]>;
    private slots: ReplaySubject<ItemSlot[]>;

    constructor(http: Http) {
        super(http);
    }

    getSectionsList(): Observable<ItemSection[]> {
        if (!this.itemSections || this.itemSections.isUnsubscribed) {
            this.itemSections = new ReplaySubject<ItemSection[]>(1);

            this._http.get('/api/item/categorylist')
                .map(res => res.json())
                .subscribe(
                    categoryList => {
                        this.itemSections.next(categoryList);
                        this.itemSections.complete();
                    },
                    error => {
                        this.itemSections.error(error);
                    }
                );
        }
        return this.itemSections;
    }

    getCategoriesById(): Observable<{[categoryId: number]: ItemCategory}> {
        let categoriesEv: EventEmitter<{[categoryId: number]: ItemCategory}> = new EventEmitter<{[categoryId: number]: ItemCategory}>();
        this.getSectionsList().subscribe(
            sections => {
                let categories: {[categoryId: number]: ItemCategory} = {};
                for (let i = 0; i < sections.length; i++) {
                    let section = sections[i];
                    for (let j = 0; j < section.categories.length; j++) {
                        let category = section.categories[j];
                        categories[category.id] = category;
                        category.type = section;
                    }
                }
                categoriesEv.emit(categories);
                categoriesEv.complete();
            },
            error => {
                this.itemSections.error(error);
            }
        );
        return categoriesEv;
    }

    getItems(section: ItemSection): Observable<ItemTemplate[]> {
        if (!(section.id in this.itemBySection) || this.itemBySection[section.id].isUnsubscribed) {

            this.itemBySection[section.id] = new ReplaySubject<ItemTemplate[]>(1);
            this.postJson('/api/item/list', {
                typeId: section.id
            }).map(res => res.json()).subscribe(
                items => {
                    this.itemBySection[section.id].next(items);
                },
                error => {
                    this.itemBySection[section.id].error(error);
                }
            );
        }
        return this.itemBySection[section.id];
    }

    create(item): Observable<ItemTemplate> {
        return this.postJson('/api/item/create', {item: item})
            .map(res => res.json());
    }

    getItem(id): Observable<ItemTemplate> {
        return this.postJson('/api/item/detail', {id: id})
            .map(res => res.json());
    }

    searchItem(filter): Observable<ItemTemplate[]> {
        return this.postJson('/api/item/search', {filter: filter})
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
        return this.postJson('/api/item/edit', {item: item})
            .map(res => res.json());
    }

    deleteItem(itemId: number): Observable<Item> {
        return this.postJson('/api/item/delete', {itemId: itemId})
            .map(res => res.json());
    }

    addItem(characterId: number
        , itemTemplateId: number
        , itemCustomName: string
        , itemCustomDescription: string
        , itemQuantity): Observable<Item> {
        return this.postJson('/api/item/add', {
            itemTemplateId: itemTemplateId,
            characterId: characterId,
            itemCustomName: itemCustomName,
            itemQuantity: itemQuantity,
            itemCustomDescription: itemCustomDescription
        }).map(res => res.json());
    }

    equipItem(itemId: number, level: number): Observable<Item> {
        return this.postJson('/api/item/equip', {
            itemId: itemId,
            level: level
        }).map(res => res.json());
    }

    moveToContainer(itemId: number, containerId: number): Observable<Item> {
        return this.postJson('/api/item/moveToContainer', {
            itemId: itemId,
            containerId: containerId
        }).map(res => res.json());
    }

    updateQuantity(itemId: number, quantity): Observable<Item> {
        return this.postJson('/api/item/updateQuantity', {
            itemId: itemId,
            quantity: quantity
        }).map(res => res.json());
    }

    readBook(itemId: number): Observable<Item> {
        return this.postJson('/api/item/readBook', {
            itemId: itemId,
        }).map(res => res.json());
    }

    updateCharge(itemId: number, charge): Observable<Item> {
        return this.postJson('/api/item/updateCharge', {
            itemId: itemId,
            charge: charge
        }).map(res => res.json());
    }

    updateItem(itemId: number, itemData): Observable<Item> {
        return this.postJson('/api/item/updateItem', {
            itemId: itemId,
            itemData: itemData
        }).map(res => res.json());
    }
}
