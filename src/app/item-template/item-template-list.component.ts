import {forkJoin, Subscription} from 'rxjs';
import {Component, EventEmitter, Input, OnDestroy, OnInit, Output} from '@angular/core';
import {animate, state, style, transition, trigger} from '@angular/animations';
import {Overlay} from '@angular/cdk/overlay';
import {ActivatedRoute, Router} from '@angular/router';

import {God, MiscService, removeDiacritics} from '../shared';

import {LoginService} from '../user';
import {OriginService} from '../origin';
import {JobService} from '../job';

import {ItemTemplate, ItemTemplateSection, ItemTemplateSubCategory} from './item-template.model';
import {ItemTemplateService} from './item-template.service';
import {CreateItemTemplateDialogComponent} from './create-item-template-dialog.component';
import {NhbkMatDialog} from '../material-workaround';
import {Guid} from '../api/shared/util';

@Component({
    selector: 'item-template-list',
    templateUrl: './item-template-list.component.html',
    styleUrls: ['item-template-list.component.scss'],
    animations: [
        trigger('stickyExpand', [
            state('0', style({
                height: '0',
                visibility: 'hidden',
                overflow: 'hidden'
            })),
            state('1', style({
                height: '*',
            })),
            transition('1 => 0', animate(200, style({height: 0, overflow: 'hidden'}))),
            transition('0 => 1', animate(200, style({height: '*', visibility: 'visible', overflow: 'hidden'})))
        ])
    ]
})
export class ItemTemplateListComponent implements OnInit, OnDestroy {
    @Input() inTab: boolean;
    @Output() onAction = new EventEmitter<{ action: string, data: any }>();
    public showHeaderInfo = false;
    public itemSections: ItemTemplateSection[];
    public selectedItemSubCategory?: ItemTemplateSubCategory;
    public previousSubCategory?: ItemTemplateSubCategory;
    public nextSubCategory?: ItemTemplateSubCategory;
    public items: ItemTemplate[] = [];
    public selectedSection?: ItemTemplateSection;
    public originsName: { [originId: string]: string };
    public jobsName: { [jobId: string]: string };
    public godsByTechName: { [techName: string]: God };

    public queryParamsSub: Subscription;

    public filter: { name?: string, dice?: number };
    public visibleItems: ItemTemplate[] = [];

    constructor(
        public readonly loginService: LoginService,
        private readonly dialog: NhbkMatDialog,
        private readonly itemTemplateService: ItemTemplateService,
        private readonly jobService: JobService,
        private readonly miscService: MiscService,
        private readonly originService: OriginService,
        private readonly overlay: Overlay,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
    ) {
        this.resetFilter();
    }

    resetNameFilter() {
        this.filter.name = undefined;
        this.updateVisibleItems();
    }

    resetDiceFilter() {
        this.filter.dice = undefined;
        this.updateVisibleItems();
    }

    resetFilter() {
        this.filter = {name: undefined, dice: undefined};
        this.updateVisibleItems();
    }

    trackById(index, element) {
        return element.id;
    }

    isVisible(item: ItemTemplate) {
        if (!this.selectedItemSubCategory) {
            return false;
        }
        if (item.subCategoryId !== this.selectedItemSubCategory.id) {
            return false;
        }
        if (item.data.diceDrop && this.filter && this.filter.dice) {
            return item.data.diceDrop === this.filter.dice;
        }
        if (this.filter && this.filter.name) {
            let cleanFilter = removeDiacritics(this.filter.name).toLowerCase();
            return removeDiacritics(item.name).toLowerCase().indexOf(cleanFilter) > -1;
        }
        return true;
    }

    updateVisibleItems() {
        this.visibleItems = this.items.filter(item => this.isVisible(item));
    }

    hasSpecial(token: string) {
        if (this.selectedSection) {
            if (this.selectedSection.specials.indexOf(token) !== -1) {
                return true;
            }
        }
        return false;
    }

    emitAction(actionName: string, data: any) {
        this.onAction.emit({action: actionName, data: data});
    }

    openCreateItemTemplateDialog() {
        const dialogRef = this.dialog.openFullScreen<CreateItemTemplateDialogComponent, any, ItemTemplate>(
            CreateItemTemplateDialogComponent
        );
        dialogRef.afterClosed().subscribe((createdItemTemplate) => {
            if (!createdItemTemplate) {
                return;
            }
            const category = this.getSubCategoryFromId(createdItemTemplate.subCategoryId);
            if (category) {
                this.reloadCategory(category[0]);
            }
        })
    }

    isEditable(itemTemplate: ItemTemplate): boolean {
        if (this.inTab) {
            return false;
        }
        if (!this.loginService.currentLoggedUser) {
            return false;
        }
        if (this.loginService.currentLoggedUser.admin) {
            return true;
        }
        if (
            itemTemplate.source !== 'official'
            && this.loginService.currentLoggedUser.id === itemTemplate.sourceUserId
        ) {
            return true;
        }
        return false;
    }

    getSubCategoryFromId(subCategoryId: number): [ItemTemplateSection, ItemTemplateSubCategory] | undefined {
        for (const itemSection of this.itemSections) {
            let itemSubCategory = itemSection.subCategories.find(x => x.id === subCategoryId);
            if (itemSubCategory) {
                return [itemSection, itemSubCategory];
            }
        }
        return undefined;
    }

    selectItemTemplate(itemTemplate: ItemTemplate) {
        let result = this.getSubCategoryFromId(itemTemplate.subCategoryId);
        if (!result) {
            return;
        }
        let [itemSection, itemSubCategory] = result;
        this.selectSection(itemSection);
        this.filter.name = itemTemplate.name;
        this.selectedItemSubCategory = itemSubCategory;
    }

    selectSectionById(sectionId: number) {
        if (this.selectedSection && this.selectedSection.id === sectionId) {
            return;
        }
        this.resetFilter();
        let i = this.itemSections.findIndex(s => s.id === sectionId);
        if (i !== -1) {
            this.selectSection(this.itemSections[i]);
        }
    }

    selectSection(category: ItemTemplateSection) {
        if (this.selectedSection && this.selectedSection.id === category.id) {
            this.reloadCategory(category);
        } else {
            this.selectedSection = category;
            this.selectSubCategory(category.subCategories[0]);
            this.resetFilter();
            this.loadSection(category);
        }
        return false;
    }

    loadSection(section: ItemTemplateSection) {
        this.selectedSection = section;
        this.itemTemplateService.getItems(section).subscribe(items => {
            this.items = items;
            this.updateVisibleItems();
        });
    }

    selectSubCategory(itemSubCategory: ItemTemplateSubCategory) {
        if (!this.selectedSection) {
            return;
        }
        this.selectedItemSubCategory = itemSubCategory;
        this.updateNextAndPreviousSubCategory();
        this.updateVisibleItems();

        const firstItem = document.getElementById('first-item');
        if (firstItem) {
            firstItem.scrollIntoView();
        }
        this.updateUrl();
    }

    private updateUrl() {
        if (!this.selectedSection) {
            return;
        }
        if (!this.selectedItemSubCategory) {
            return;
        }
        if (!this.inTab) {
            this.router.navigate(['items', this.selectedSection.id, this.selectedItemSubCategory.id], {
                relativeTo: this.route.parent,
                replaceUrl: true
            });
        }
    }

    private updateNextAndPreviousSubCategory() {
        if (!this.selectedItemSubCategory) {
            return;
        }
        if (!this.selectedSection) {
            return;
        }

        const index = this.selectedSection.subCategories.indexOf(this.selectedItemSubCategory);
        let previousIndex = (index - 1 + this.selectedSection.subCategories.length) % this.selectedSection.subCategories.length;
        this.previousSubCategory = this.selectedSection.subCategories[previousIndex];
        let nextIndex = (index + 1) % this.selectedSection.subCategories.length;
        this.nextSubCategory = this.selectedSection.subCategories[nextIndex];
    }

    selectCategoryById(categoryId: number, subCategoryId: number) {
        if (this.selectedSection && this.selectedSection.id === categoryId) {
            return;
        }
        if (this.selectedItemSubCategory && this.selectedItemSubCategory.id === subCategoryId) {
            return;
        }
        const category = this.itemSections.find(x => x.id === categoryId);
        if (!category) {
            this.selectSection(this.itemSections[0]);
            return;
        }
        let subCategory = category.subCategories.find(x => x.id === subCategoryId);
        if (!subCategory) {
            subCategory = category.subCategories[0];
        }

        if (!this.selectedSection || category.id !== this.selectedSection.id) {
            this.loadSection(category);
        }

        this.selectedSection = category;
        this.selectedItemSubCategory = subCategory;
        this.updateNextAndPreviousSubCategory();
        this.updateUrl();
    }

    navigateToItem(itemId: Guid): void {
        if (this.inTab) {
            return;
        }
        this.router.navigate([], {preserveFragment: true, fragment: 'item-' + itemId, relativeTo: this.route});
    }

    reloadSectionForItem(itemTemplate: ItemTemplate) {
        const category = this.getSubCategoryFromId(itemTemplate.subCategoryId);
        if (category) {
            this.reloadCategory(category[0]);
        }
    }

    private reloadCategory(category: ItemTemplateSection) {
        this.itemTemplateService.clearItemSectionCache(category.id);
        this.loadSection(category);
    }

    ngOnInit() {
        forkJoin([
            this.jobService.getJobsNamesById(),
            this.originService.getOriginsNamesById(),
            this.itemTemplateService.getSectionsList(),
            this.miscService.getGodsByTechName(),
        ]).subscribe(([jobsName, originsName, sections, godsByTechName]) => {
            this.originsName = originsName;
            this.jobsName = jobsName;
            this.itemSections = sections;
            this.godsByTechName = godsByTechName;
            if (!this.route.snapshot.params['categoryId']) {
                this.selectSection(sections[0]);
            }
            this.queryParamsSub = this.route.paramMap.subscribe(params => {
                if (params.has('categoryId') && params.has('subCategoryId')) {
                    this.selectCategoryById(+params.get('categoryId')!, +params.get('subCategoryId')!);
                } else if (params.has('categoryId')) {
                    this.selectSectionById(+params.get('categoryId')!);
                }
            });
        });
    }

    ngOnDestroy() {
        if (this.queryParamsSub) {
            this.queryParamsSub.unsubscribe();
        }
    }
}

