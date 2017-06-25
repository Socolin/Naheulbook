import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';

import {ItemTemplate, ItemSection, ItemSlot} from '../item';
import {Item} from '../character';
import {JsonService} from '../shared/json-service';
import {ItemCategory, ItemTemplateJsonData} from './item-template.model';
import {NotificationsService} from '../notifications';
import {ItemData, PartialItem} from '../character';
import {LoginService} from '../user';
import {SkillService} from '../skill';
import {Skill} from '../skill';
import {LootTookItemMsg} from '../loot/loot.model';
import {ActiveStatsModifier} from '../shared/stat-modifier.model';

@Injectable()
export class ItemService extends JsonService {
    private itemBySection: {[sectionId: number]: ReplaySubject<ItemTemplate[]>} = {};
    private itemSections: ReplaySubject<ItemSection[]>;
    private slots: ReplaySubject<ItemSlot[]>;

    constructor(http: Http
        , notification: NotificationsService
        , private _skillService: SkillService
        , loginService: LoginService) {
        super(http, notification, loginService);
    }

    getSectionsList(): Observable<ItemSection[]> {
        if (!this.itemSections) {
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
        return this.getSectionsList().map(sections => {
            let categories: {[categoryId: number]: ItemCategory} = {};
            for (let i = 0; i < sections.length; i++) {
                let section = sections[i];
                for (let j = 0; j < section.categories.length; j++) {
                    let category = section.categories[j];
                    categories[category.id] = category;
                    category.type = section;
                }
            }
            return categories;
        });
    }

    getItems(section: ItemSection): Observable<ItemTemplate[]> {
        if (!(section.id in this.itemBySection)) {

            this.itemBySection[section.id] = new ReplaySubject<ItemTemplate[]>(1);
            Observable.forkJoin(
                this.postJson('/api/item/list', {typeId: section.id}).map(res => res.json()),
                this._skillService.getSkillsById()
            ).subscribe(
                ([itemTemplateDatas, skillsById]: [ItemTemplateJsonData[], {[skillId: number]: Skill}]) => {
                    let items: ItemTemplate[] = [];
                    for (let i = 0; i < itemTemplateDatas.length; i++) {
                        let itemTemplate = ItemTemplate.fromJson(itemTemplateDatas[i], skillsById);
                        items.push(itemTemplate);
                    }
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

    getItem(id: number): Observable<ItemTemplate> {
        return Observable.create((function (observer) {
            Observable.forkJoin(
                this.postJson('/api/item/detail', {id: id}).map(res => res.json()),
                this._skillService.getSkillsById()
            ).subscribe(
                ([itemTemplateData, skillsById]: [ItemTemplateJsonData, {[skillId: number]: Skill}]) => {
                    let itemTemplate = ItemTemplate.fromJson(itemTemplateData, skillsById);
                    observer.next(itemTemplate);
                    observer.complete();
                },
                error => {
                    observer.error(error);
                }
            );
        }).bind(this));
    }

    searchItem(filter): Observable<ItemTemplate[]> {
        if (!filter) {
            return Observable.from([]);
        }
        return this.postJson('/api/item/search', {filter: filter})
            .map(res => res.json());
    }

    getSlots(): Observable<ItemSlot[]> {
        if (!this.slots) {
            this.slots = new ReplaySubject<ItemSlot[]>(1);

            this._http.get('/api/item/slotList')
                .map(res => res.json())
                .subscribe(
                    categoryList => {
                        this.slots.next(categoryList);
                        this.slots.complete();
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

    takeItemFromLoot(itemId: number, characterId: number, quantity?: number): Observable<LootTookItemMsg> {
        return this.postJson('/api/item/takeItemFromLoot', {
            itemId: itemId,
            quantity: quantity,
            characterId: characterId
        }).map(res => res.json());
    }

    giveItem(itemId: number, characterId: number, quantity?: number): Observable<Item> {
        return this.postJson('/api/item/giveItem', {itemId: itemId, characterId: characterId, quantity: quantity})
            .map(res => res.json());
    }

    addItemTo(targetType: string
        , targetId: number
        , itemTemplateId: number
        , itemData: ItemData): Observable<Item> {
        if (itemData.quantity) {
            itemData.quantity = +itemData.quantity;
        } else {
            delete itemData['quantity'];
        }

        return Observable.create(observer => {
            Observable.forkJoin(this.postJson('/api/item/add', {
                    itemTemplateId: itemTemplateId,
                    targetId: targetId,
                    targetType: targetType,
                    itemData: itemData
                }).map(res => res.json()),
                this._skillService.getSkillsById()
            ).subscribe(
                ([itemJsonData, skillsById]: [Item, {[skillId: number]: Skill}]) => {
                    let item = Item.fromJson(itemJsonData, skillsById);
                    observer.next(item);
                    observer.complete();
                },
                err => {
                    observer.error(err);
                    observer.complete();
                }
            );
        });
    }
    addRandomItemTo(targetType: string
        , targetId: number
        , criteria: any): Observable<Item> {

        return Observable.create(observer => {
            Observable.forkJoin(this.postJson('/api/item/addRandom', {
                    targetId: targetId,
                    targetType: targetType,
                    criteria: criteria
                }).map(res => res.json()),
                this._skillService.getSkillsById()
            ).subscribe(
                ([itemJsonData, skillsById]: [Item, {[skillId: number]: Skill}]) => {
                    let item = Item.fromJson(itemJsonData, skillsById);
                    observer.next(item);
                    observer.complete();
                },
                err => {
                    observer.error(err);
                    observer.complete();
                }
            );
        });
    }

    clearItemSectionCache(sectionId: number) {
        delete this.itemBySection[sectionId];
    }

    getSectionFromCategory(categoryId: number): Observable<ItemSection> {
        return Observable.create(observer => {
            this.getSectionsList().subscribe(
                sections => {
                    for (let i = 0; i < sections.length; i++) {
                        let section = sections[i];
                        for (let j = 0; j < section.categories.length; j++) {
                            if (categoryId === section.categories[j].id) {
                                observer.next(section);
                                observer.complete();
                                return;
                            }
                        }
                    }
                    observer.next(null);
                    observer.complete();
                    return;
                }
            );
        });
    }


    equipItem(itemId: number, level: number): Observable<PartialItem> {
        return this.postJson('/api/item/equip', {
            itemId: itemId,
            level: level
        }).map(res => res.json());
    }

    moveToContainer(itemId: number, containerId: number): Observable<PartialItem> {
        return this.postJson('/api/item/moveToContainer', {
            itemId: itemId,
            containerId: containerId
        }).map(res => res.json());
    }

    updateQuantity(itemId: number, quantity): Observable<PartialItem> {
        return this.postJson('/api/item/updateQuantity', {
            itemId: itemId,
            quantity: quantity
        }).map(res => res.json());
    }

    readBook(itemId: number): Observable<PartialItem> {
        return this.postJson('/api/item/readBook', {
            itemId: itemId,
        }).map(res => res.json());
    }

    identify(itemId: number): Observable<PartialItem> {
        return this.postJson('/api/item/identify', {
            itemId: itemId,
        }).map(res => res.json());
    }

    updateCharge(itemId: number, charge): Observable<PartialItem> {
        return this.postJson('/api/item/updateCharge', {
            itemId: itemId,
            charge: charge
        }).map(res => res.json());
    }

    updateItem(itemId: number, itemData): Observable<PartialItem> {
        return this.postJson('/api/item/updateItem', {
            itemId: itemId,
            itemData: itemData
        }).map(res => res.json());
    }

    updateItemModifiers(itemId: number, modifiers: ActiveStatsModifier[]): Observable<PartialItem> {
        return this.postJson('/api/item/updateItemModifiers', {
            itemId: itemId,
            modifiers: modifiers
        }).map(res => res.json());
    }

    getGem(type: string, dice: number): Observable<ItemTemplate> {
        return this.postJson('/api/item/getGem', {
            type: type,
            dice: dice
        }).map(res => res.json());
    }
}
