import {from as observableFrom, forkJoin, ReplaySubject, Observable} from 'rxjs';

import {map, withLatestFrom} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {Skill, SkillDictionary, SkillService} from '../skill';

import {
    ItemTemplate,
    ItemTemplateSection,
    ItemSlot,
    ItemTemplateCategoryDictionary
} from './item-template.model';
import {ItemSlotResponse, ItemTemplateResponse, ItemTemplateSectionResponse, ItemTypeResponse} from '../api/responses';
import {ItemTemplateRequest} from '../api/requests/item-template-request';

@Injectable()
export class ItemTemplateService {
    private itemBySection: { [sectionId: number]: ReplaySubject<ItemTemplate[]> } = {};
    private itemsByCategoriesName: { [categoryName: string]: ReplaySubject<ItemTemplate[]> } = {};
    private itemSections?: ReplaySubject<ItemTemplateSection[]>;
    private slots?: ReplaySubject<ItemSlot[]>;
    private itemTypes?: ReplaySubject<ItemTypeResponse[]>;

    constructor(
        private readonly httpClient: HttpClient,
        private readonly skillService: SkillService,
    ) {
    }

    getSectionsList(): Observable<ItemTemplateSection[]> {
        if (!this.itemSections) {
            const loadingItemSections = new ReplaySubject<ItemTemplateSection[]>(1);
            this.itemSections = loadingItemSections;
            this.httpClient.get<ItemTemplateSectionResponse[]>('/api/v2/itemTemplateSections')
                .subscribe(
                    itemSectionResponses => {
                        const itemTemplateSections = ItemTemplateSection.fromResponses(itemSectionResponses);
                        loadingItemSections.next(itemTemplateSections);
                        loadingItemSections.complete();
                    },
                    error => {
                        loadingItemSections.error(error);
                    }
                );
        }
        return this.itemSections;
    }

    getCategoriesById(): Observable<ItemTemplateCategoryDictionary> {
        return this.getSectionsList().pipe(map(itemTemplateSections => {
            let categoriesByIds: ItemTemplateCategoryDictionary = {};
            for (const sectionCategories of itemTemplateSections.map(section => section.categories)) {
                categoriesByIds = sectionCategories.reduce((sectionCategoriesById, category) => {
                    categoriesByIds[category.id] = category;
                    return sectionCategoriesById
                }, categoriesByIds);
            }
            return categoriesByIds;
        }));
    }

    getItems(section: ItemTemplateSection): Observable<ItemTemplate[]> {
        if (!(section.id in this.itemBySection)) {
            this.itemBySection[section.id] = new ReplaySubject<ItemTemplate[]>(1);
            forkJoin([
                this.httpClient.get('/api/v2/itemTemplateSections/' + section.id),
                this.skillService.getSkillsById()
            ]).subscribe(
                ([itemTemplateDatas, skillsById]: [ItemTemplateResponse[], SkillDictionary]) => {
                    let items: ItemTemplate[] = [];
                    for (let i = 0; i < itemTemplateDatas.length; i++) {
                        let itemTemplate = ItemTemplate.fromResponse(itemTemplateDatas[i], skillsById);
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

    createItemTemplate(request: ItemTemplateRequest): Observable<ItemTemplate> {
        return this.httpClient.post<ItemTemplateResponse>('/api/v2/itemTemplates/', request)
            .pipe(
                withLatestFrom(this.skillService.getSkillsById()),
                map(([response, skillsById]) => ItemTemplate.fromResponse(response, skillsById))
            );
    }

    editItemTemplate(itemTemplateId: number, request: ItemTemplateRequest): Observable<ItemTemplate> {
        return this.httpClient.put<ItemTemplateResponse>(`/api/v2/itemTemplates/${itemTemplateId}`, request)
            .pipe(
                withLatestFrom(this.skillService.getSkillsById()),
                map(([response, skillsById]) => ItemTemplate.fromResponse(response, skillsById))
            );
    }

    getItem(id: number): Observable<ItemTemplate> {
        return new Observable((observer) => {
            forkJoin([
                this.httpClient.get<ItemTemplateResponse>(`/api/v2/itemTemplates/${id}`),
                this.skillService.getSkillsById()
            ]).subscribe(
                ([itemTemplateData, skillsById]: [ItemTemplateResponse, { [skillId: number]: Skill }]) => {
                    let itemTemplate = ItemTemplate.fromResponse(itemTemplateData, skillsById);
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
            this.httpClient.get<ItemTemplateResponse[]>('/api/v2/itemTemplates/search?filter=' + encodeURIComponent(filter)),
            this.skillService.getSkillsById()
        ]).pipe(map(([itemTemplateDatas, skillsById]: [ItemTemplateResponse[], SkillDictionary]) => {
            return ItemTemplate.fromResponses(itemTemplateDatas, skillsById);
        }));
    }

    getSlots(): Observable<ItemSlot[]> {
        if (!this.slots) {
            const loadingSlots = new ReplaySubject<ItemSlot[]>(1);
            this.slots = loadingSlots;

            this.httpClient.get<ItemSlotResponse[]>('/api/v2/itemSlots')
                .subscribe(
                    slots => {
                        loadingSlots.next(slots);
                        loadingSlots.complete();
                    },
                    error => {
                        loadingSlots.error(error);
                    }
                );
        }
        return this.slots;
    }

    getItemTypes(): Observable<ItemTypeResponse[]> {
        if (!this.itemTypes) {
            const loadingItemTypes = new ReplaySubject<ItemTypeResponse[]>(1);
            this.itemTypes = loadingItemTypes;

            this.httpClient.get<ItemTypeResponse[]>('/api/v2/itemTypes')
                .subscribe(
                    itemTypes => {
                        loadingItemTypes.next(itemTypes);
                        loadingItemTypes.complete();
                    },
                    error => {
                        loadingItemTypes.error(error);
                    }
                );
        }
        return this.itemTypes;
    }

    createItemType(displayName: string, techName: string): Observable<ItemTypeResponse> {
        this.itemTypes = undefined;
        let itemTypeRequest = {
            techName: techName,
            displayName: displayName
        };
        return this.httpClient.post<ItemTypeResponse>('/api/v2/itemTypes', itemTypeRequest);
    }

    clearItemSectionCache(sectionId: number) {
        delete this.itemBySection[sectionId];
    }

    getItemTemplatesByCategoryTechName(categoryTechName: string): Observable<ItemTemplate[]> {
        if (!(categoryTechName in this.itemsByCategoriesName)) {
            this.itemsByCategoriesName[categoryTechName] = new ReplaySubject<ItemTemplate[]>(1);

            forkJoin([
                this.httpClient.get<ItemTemplateResponse[]>(`/api/v2/itemTemplateCategories/${categoryTechName}/itemTemplates`),
                this.skillService.getSkillsById()
            ]).pipe(
                map(([itemTemplatesData, skillsById]: [ItemTemplateResponse[], { [skillId: number]: Skill }]) => {
                    let items: ItemTemplate[] = [];
                    for (let i = 0; i < itemTemplatesData.length; i++) {
                        let itemTemplate = ItemTemplate.fromResponse(itemTemplatesData[i], skillsById);
                        items.push(itemTemplate);
                    }
                    return items;
                })
            ).subscribe(itemTemplates => {
                this.itemsByCategoriesName[categoryTechName].next(itemTemplates);
            }, error => {
                this.itemsByCategoriesName[categoryTechName].error(error);
            });
        }

        return this.itemsByCategoriesName[categoryTechName];
    }
}
