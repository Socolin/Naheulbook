import {from as observableFrom, forkJoin, ReplaySubject, Observable} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {Skill, SkillService} from '../skill';

import {ItemCategory, ItemTemplateJsonData, ItemTemplate, ItemSection, ItemSlot, ItemType} from './item-template.model';

@Injectable()
export class ItemTemplateService {
    private itemBySection: { [sectionId: number]: ReplaySubject<ItemTemplate[]> } = {};
    private itemSections?: ReplaySubject<ItemSection[]>;
    private slots?: ReplaySubject<ItemSlot[]>;
    private itemTypes?: ReplaySubject<ItemType[]>;

    constructor(private httpClient: HttpClient
        , private _skillService: SkillService
    ) {
    }

    getSectionsList(): Observable<ItemSection[]> {
        if (!this.itemSections) {
            this.itemSections = new ReplaySubject<ItemSection[]>(1);

            this.httpClient.get<ItemSection[]>('/api/v2/itemTemplateSections')
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

    getCategoriesById(): Observable<{ [categoryId: number]: ItemCategory }> {
        return this.getSectionsList().pipe(map(sections => {
            let categories: { [categoryId: number]: ItemCategory } = {};
            for (let i = 0; i < sections.length; i++) {
                let section = sections[i];
                for (let j = 0; j < section.categories.length; j++) {
                    let category = section.categories[j];
                    categories[category.id] = category;
                    category.section = section;
                }
            }
            return categories;
        }));
    }

    getItems(section: ItemSection): Observable<ItemTemplate[]> {
        if (!(section.id in this.itemBySection)) {
            this.itemBySection[section.id] = new ReplaySubject<ItemTemplate[]>(1);
            forkJoin([
                this.httpClient.get('/api/v2/itemTemplateSections/' + section.id),
                this._skillService.getSkillsById()
            ]).subscribe(
                ([itemTemplateDatas, skillsById]: [ItemTemplateJsonData[], { [skillId: number]: Skill }]) => {
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
        return this.httpClient.post<ItemTemplate>('/api/itemtemplate/create', {item: item})
    }

    getItem(id: number): Observable<ItemTemplate> {
        return new Observable((observer) => {
            forkJoin([
                this.httpClient.post('/api/itemtemplate/detail', {id: id}),
                this._skillService.getSkillsById()
            ]).subscribe(
                ([itemTemplateData, skillsById]: [ItemTemplateJsonData, { [skillId: number]: Skill }]) => {
                    let itemTemplate = ItemTemplate.fromJson(itemTemplateData, skillsById);
                    observer.next(itemTemplate);
                    observer.complete();
                },
                error => {
                    observer.error(error);
                }
            );
        });
    }

    searchItem(filter): Observable<ItemTemplate[]> {
        if (!filter) {
            return observableFrom([]);
        }

        return forkJoin([
            this.httpClient.post('/api/itemtemplate/search', {filter: filter}),
            this._skillService.getSkillsById()
        ]).pipe(map(([itemTemplateDatas, skillsById]: [ItemTemplateJsonData[], { [skillId: number]: Skill }]) => {
            return ItemTemplate.itemTemplatesFromJson(itemTemplateDatas, skillsById);
        }));
    }

    getSlots(): Observable<ItemSlot[]> {
        if (!this.slots) {
            this.slots = new ReplaySubject<ItemSlot[]>(1);

            this.httpClient.get<ItemSlot[]>('/api/itemtemplate/slotList')
                .subscribe(
                    slots => {
                        this.slots.next(slots);
                        this.slots.complete();
                    },
                    error => {
                        this.slots.error(error);
                    }
                );
        }
        return this.slots;
    }

    getItemTypes(): Observable<ItemType[]> {
        if (!this.itemTypes) {
            this.itemTypes = new ReplaySubject<ItemType[]>(1);

            this.httpClient.get<ItemType[]>('/api/itemtemplate/itemTypesList')
                .subscribe(
                    itemTypes => {
                        this.itemTypes.next(itemTypes);
                        this.itemTypes.complete();
                    },
                    error => {
                        this.itemTypes.error(error);
                    }
                );
        }
        return this.itemTypes;
    }

    createItemType(displayName: string, techName: string): Observable<ItemType> {
        this.itemTypes = undefined;
        let itemType = {
            techName: techName,
            displayName: displayName
        };
        return this.httpClient.post<ItemType>('/api/itemtemplate/createItemType', {itemType: itemType})
    }

    editItemTemplate(item): Observable<ItemTemplate> {
        return this.httpClient.post<ItemTemplate>('/api/itemtemplate/edit', {item: item})
    }

    clearItemSectionCache(sectionId: number) {
        delete this.itemBySection[sectionId];
    }

    getSectionFromCategory(categoryId: number): Observable<ItemSection> {
        return new Observable(observer => {
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
        return this.httpClient.post<ItemTemplate>('/api/itemtemplate/getGem', {
            type: type,
            dice: dice
        });
    }
}
