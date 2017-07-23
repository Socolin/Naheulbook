import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';

import {JsonService} from '../shared';
import {NotificationsService} from '../notifications';

import {LoginService} from '../user';
import {Skill, SkillService} from '../skill';

import {ItemCategory, ItemTemplateJsonData, ItemTemplate, ItemSection, ItemSlot} from '.';

@Injectable()
export class ItemTemplateService extends JsonService {
    private itemBySection: {[sectionId: number]: ReplaySubject<ItemTemplate[]>} = {};
    private itemSections: ReplaySubject<ItemSection[]>;
    private slots: ReplaySubject<ItemSlot[]>;

    constructor(http: Http
        , notification: NotificationsService
        , loginService: LoginService
        , private _skillService: SkillService
    ) {
        super(http, notification, loginService);
    }

    getSectionsList(): Observable<ItemSection[]> {
        if (!this.itemSections) {
            this.itemSections = new ReplaySubject<ItemSection[]>(1);

            this._http.get('/api/itemtemplate/categorylist')
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
                this.postJson('/api/itemtemplate/list', {typeId: section.id}).map(res => res.json()),
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
        return this.postJson('/api/itemtemplate/create', {item: item})
            .map(res => res.json());
    }

    getItem(id: number): Observable<ItemTemplate> {
        return Observable.create((function (observer) {
            Observable.forkJoin(
                this.postJson('/api/itemtemplate/detail', {id: id}).map(res => res.json()),
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

        return Observable.forkJoin(
            this.postJson('/api/itemtemplate/search', {filter: filter}).map(res => res.json()),
            this._skillService.getSkillsById()
        ).map(([itemTemplateDatas, skillsById]: [ItemTemplateJsonData[], {[skillId: number]: Skill}]) => {
            return ItemTemplate.itemTemplatesFromJson(itemTemplateDatas, skillsById);
        });
    }

    getSlots(): Observable<ItemSlot[]> {
        if (!this.slots) {
            this.slots = new ReplaySubject<ItemSlot[]>(1);

            this._http.get('/api/itemtemplate/slotList')
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
        return this.postJson('/api/itemtemplate/edit', {item: item})
            .map(res => res.json());
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

    getGem(type: string, dice: number): Observable<ItemTemplate> {
        return this.postJson('/api/itemtemplate/getGem', {
            type: type,
            dice: dice
        }).map(res => res.json());
    }
}
