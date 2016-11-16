import {Injectable, EventEmitter} from '@angular/core';
import {Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';

import {ItemTemplate, ItemSection, ItemSlot} from '../item';
import {Item} from "../character";
import {JsonService} from '../shared/json-service';
import {ItemCategory} from './item-template.model';
import {NotificationsService} from '../notifications';
import {ItemData, PartialItem} from "../character";
import {LoginService} from "../user";
import {SkillService} from "../skill";
import {Skill} from "../skill";
import {ItemModifier} from "../character/item.model";

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

    private fillMissingDataInItemTemplate(itemTemplate: ItemTemplate, skillsById: {[skillId: number]: Skill}) {
        for (let j = 0; j < itemTemplate.skills.length; j++) {
            let partialSkill = itemTemplate.skills[j];
            itemTemplate.skills[j] = skillsById[partialSkill.id];
        }

        for (let j = 0; j < itemTemplate.unskills.length; j++) {
            let partialSkill = itemTemplate.unskills[j];
            itemTemplate.unskills[j] = skillsById[partialSkill.id];
        }

        for (let j = 0; j < itemTemplate.skillModifiers.length; j++) {
            let skillModifier = itemTemplate.skillModifiers[j];
            skillModifier.skill = skillsById[+skillModifier.skill];
        }
    }

    getItems(section: ItemSection): Observable<ItemTemplate[]> {
        if (!(section.id in this.itemBySection)) {

            this.itemBySection[section.id] = new ReplaySubject<ItemTemplate[]>(1);
            Observable.forkJoin(
                this.postJson('/api/item/list', {typeId: section.id}).map(res => res.json()),
                this._skillService.getSkillsById()
            ).subscribe(
                res => {
                    let items : ItemTemplate[] = res[0];
                    let skillsById: {[skillId: number]: Skill} = res[1];
                    for (let i = 0; i < items.length; i++) {
                        let item = items[i];
                        this.fillMissingDataInItemTemplate(item, skillsById);
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

    getItem(id): Observable<ItemTemplate> {
        return this.postJson('/api/item/detail', {id: id})
            .map(res => res.json());
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

    takeItemFromLoot(itemId: number, characterId: number): Observable<Item> {
        return this.postJson('/api/item/takeItemFromLoot', {
            itemId: itemId,
            characterId: characterId
        }).map(res => res.json());
    }

    giveItem(itemId: number, characterId: number): Observable<Item> {
        return this.postJson('/api/item/giveItem', {itemId: itemId, characterId: characterId})
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
                res => {
                    let item: Item = res[0];
                    let skillsById: {[skillId: number]: Skill} = res[1];
                    this.fillMissingDataInItemTemplate(item.template, skillsById);
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

    updateItemModifiers(itemId: number, modifiers: ItemModifier[]): Observable<PartialItem> {
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
