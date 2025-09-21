import {forkJoin, from as observableFrom, Observable, ReplaySubject} from 'rxjs';

import {map, withLatestFrom} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {Skill, SkillDictionary, SkillService} from '../skill';

import {ItemSlot, ItemTemplate, ItemTemplateSection, ItemTemplateSubCategoryDictionary} from './item-template.model';
import {ItemSlotResponse, ItemTemplateResponse, ItemTemplateSectionResponse, ItemTypeResponse} from '../api/responses';
import {ItemTemplateRequest} from '../api/requests';
import {Guid} from '../api/shared/util';

@Injectable({providedIn: 'root'})
export class ItemTemplateService {
    private itemBySection: { [sectionId: number]: ReplaySubject<ItemTemplate[]> } = {};
    private itemsBySubCategoriesName: { [subCategoryName: string]: ReplaySubject<ItemTemplate[]> } = {};
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

    getSubCategoriesById(): Observable<ItemTemplateSubCategoryDictionary> {
        return this.getSectionsList().pipe(map(itemTemplateSections => {
            let categoriesByIds: ItemTemplateSubCategoryDictionary = {};
            for (const subCategories of itemTemplateSections.map(section => section.subCategories)) {
                categoriesByIds = subCategories.reduce((sectionCategoriesById, subCategory) => {
                    categoriesByIds[subCategory.id] = subCategory;
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

    editItemTemplate(itemTemplateId: Guid, request: ItemTemplateRequest): Observable<ItemTemplate> {
        return this.httpClient.put<ItemTemplateResponse>(`/api/v2/itemTemplates/${itemTemplateId}`, request)
            .pipe(
                withLatestFrom(this.skillService.getSkillsById()),
                map(([response, skillsById]) => ItemTemplate.fromResponse(response, skillsById))
            );
    }

    getItem(itemTemplateId: Guid): Observable<ItemTemplate> {
        return new Observable((observer) => {
            forkJoin([
                this.httpClient.get<ItemTemplateResponse>(`/api/v2/itemTemplates/${itemTemplateId}`),
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

    searchItem(filter: string): Observable<ItemTemplate[]> {
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

    getItemTemplatesBySubCategoryTechName(subCategoryTechName: string): Observable<ItemTemplate[]> {
        if (!(subCategoryTechName in this.itemsBySubCategoriesName)) {
            this.itemsBySubCategoriesName[subCategoryTechName] = new ReplaySubject<ItemTemplate[]>(1);

            forkJoin([
                this.httpClient.get<ItemTemplateResponse[]>(`/api/v2/itemTemplateSubCategories/${subCategoryTechName}/itemTemplates`),
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
                this.itemsBySubCategoriesName[subCategoryTechName].next(itemTemplates);
            }, error => {
                this.itemsBySubCategoriesName[subCategoryTechName].error(error);
            });
        }

        return this.itemsBySubCategoriesName[subCategoryTechName];
    }
}
