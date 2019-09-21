import {forkJoin, Subscription} from 'rxjs';
import {Component, EventEmitter, Input, OnDestroy, OnInit, Output, QueryList, ViewChildren} from '@angular/core';
import {animate, state, style, transition, trigger} from '@angular/animations';
import {Overlay} from '@angular/cdk/overlay';
import {ActivatedRoute, Router} from '@angular/router';

import {God, MiscService, removeDiacritics} from '../shared';

import {LoginService} from '../user';
import {OriginService} from '../origin';
import {JobService} from '../job';

import {ItemCategory, ItemSection, ItemTemplate} from './item-template.model';
import {ItemTemplateService} from './item-template.service';
import {ItemCategoryDirective} from './item-category.directive';
import {MatDialog} from '@angular/material';
import {CreateItemTemplateDialogComponent} from './create-item-template-dialog.component';

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
            state('1',   style({
                height: '*',
            })),
            transition('1 => 0', animate(200, style({height: 0, overflow: 'hidden'}))),
            transition('0 => 1', animate(200, style({height: '*', visibility: 'visible', overflow: 'hidden'})))
        ])
    ]
})
export class ItemTemplateListComponent implements OnInit, OnDestroy {
    @Input() inTab: boolean;
    @Output() onAction = new EventEmitter<{action: string, data: any}>();
    public showHeaderInfo = false;
    public itemSections: ItemSection[];
    public selectedItemCategory?: ItemCategory;
    public previousSubCategory?: ItemCategory;
    public nextSubCategory?: ItemCategory;
    public items: ItemTemplate[] = [];
    public selectedSection: ItemSection;
    public originsName: {[originId: number]: string};
    public jobsName: {[jobId: number]: string};
    public godsByTechName: {[techName: string]: God};

    public queryParamsSub: Subscription;

    @ViewChildren(ItemCategoryDirective)
    public stickToTopElements: QueryList<ItemCategoryDirective>;

    public filter: {name?: string, dice?: number};
    public visibleItems: ItemTemplate[] = [];

    constructor(private _router: Router
        , public overlay: Overlay
        , public dialog: MatDialog
        , private _route: ActivatedRoute
        , public _loginService: LoginService
        , private _miscService: MiscService
        , private _itemTemplateService: ItemTemplateService
        , private _originService: OriginService
        , private _jobService: JobService) {
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

    selectSection(section: ItemSection) {
        if (this.selectedSection && this.selectedSection.id === section.id) {
            this._itemTemplateService.clearItemSectionCache(section.id);
            this.loadSection(section);
        } else {
            if (!this.inTab) {
                this._router.navigate(['../items'], {queryParams: {id: section.id}, relativeTo: this._route});
            }

            this.selectedSection = section;
            this.selectCategory(section.categories[0]);
            this.resetFilter();
            this.loadSection(section);
        }
        return false;
    }

    trackById(index, element) {
        return element.id;
    }

    isVisible(item: ItemTemplate) {
        if (item.categoryId !== this.selectedItemCategory.id) {
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

    loadSection(section: ItemSection) {
        this.selectedSection = section;
        this._itemTemplateService.getItems(section).subscribe(items => {
            this.items = items;
            this.updateVisibleItems();
        });
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

    getCategoryFromId(categoryId: number): [ItemSection, ItemCategory ]| undefined {
        for (let ci = 0; ci < this.itemSections.length; ci++) {
            let itemSection = this.itemSections[ci];
            for (let i = 0; i < itemSection.categories.length; i++) {
                let itemCategory = itemSection.categories[i];
                if (itemCategory.id === categoryId) {
                    return [itemSection, itemCategory];
                }
            }
        }
        return undefined;
    }

    selectItemTemplate(itemTemplate: ItemTemplate) {
        let result = this.getCategoryFromId(itemTemplate.categoryId);
        if (!result) {
            return;
        }
        let [itemSection, itemCategory] = result;
        this.selectSection(itemSection);
        this.filter.name = itemTemplate.name;
        this.selectedItemCategory = itemCategory;
    }

    openCreateItemTemplateDialog() {
        this.dialog.open(CreateItemTemplateDialogComponent, {minWidth: '100vw', height: '100vh'});
    }

    isEditable(itemTemplate: ItemTemplate): boolean {
        if (this.inTab) {
            return false;
        }
        if (!this._loginService.currentLoggedUser) {
            return false;
        }
        if (this._loginService.currentLoggedUser.admin) {
            return true;
        }
        if (itemTemplate.source !== 'official'
            && this._loginService.currentLoggedUser.id === itemTemplate.sourceUserId) {
            return true;
        }
        return false;
    }

    ngOnInit() {
        forkJoin([
            this._jobService.getJobsNamesById(),
            this._originService.getOriginsNamesById(),
            this._itemTemplateService.getSectionsList(),
            this._miscService.getGodsByTechName(),
        ]).subscribe(([jobsName, originsName, sections, godsByTechName]) => {
            this.originsName = originsName;
            this.jobsName = jobsName;
            this.itemSections = sections;
            this.godsByTechName = godsByTechName;
            if (!this._route.snapshot.queryParams['id']) {
                this.selectSection(sections[0]);
            }
            this.queryParamsSub = this._route.queryParams.subscribe(params => {
                this.selectSectionById(+params['id']);
            });
        });
    }

    ngOnDestroy() {
        if (this.queryParamsSub) {
            this.queryParamsSub.unsubscribe();
        }
    }

    selectCategory(itemCategory: ItemCategory) {
        this.selectedItemCategory = itemCategory;
        const index = this.selectedSection.categories.indexOf(itemCategory);
        let previousIndex = (index - 1 + this.selectedSection.categories.length) % this.selectedSection.categories.length;
        this.previousSubCategory = this.selectedSection.categories[previousIndex];
        let nextIndex = (index + 1) % this.selectedSection.categories.length;
        this.nextSubCategory = this.selectedSection.categories[nextIndex];
        this.updateVisibleItems();
        document.getElementById('first-item').scrollIntoView();
    }
}

