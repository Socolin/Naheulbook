import {forkJoin, Subscription} from 'rxjs';
import {
    Component, OnInit, OnDestroy, Input, ViewChildren, HostListener, QueryList, ViewChild, EventEmitter, Output
} from '@angular/core';
import {animate, state, style, transition, trigger} from '@angular/animations';
import {Overlay, OverlayRef, OverlayConfig} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';
import {ActivatedRoute, Router} from '@angular/router';

import {smoothScrollTo} from '../shared/scroll';
import {God, removeDiacritics} from '../shared';

import {LoginService} from '../user';
import {OriginService} from '../origin';
import {JobService} from '../job';

import {ItemCategory, ItemTemplate} from './item-template.model';
import {ItemTemplateService} from './item-template.service';
import {ItemCategoryDirective} from './item-category.directive';
import {ItemSection} from './item-template.model';
import {MiscService} from '../shared/misc.service';
import {MatDialog} from '@angular/material';
import {CreateItemTemplateDialogComponent} from './create-item-template-dialog.component';

@Component({
    selector: 'item-list',
    templateUrl: './item-list.component.html',
    styleUrls: ['item-list.component.scss'],
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
export class ItemListComponent implements OnInit, OnDestroy {
    @Input() inTab: boolean;
    @Output() onAction = new EventEmitter<{action: string, data: any}>();
    public itemSections: ItemSection[];
    public items: ItemTemplate[] = [];
    public selectedSection: ItemSection;
    public originsName: {[originId: number]: string};
    public jobsName: {[jobId: number]: string};
    public godsByTechName: {[techName: string]: God};

    public queryParamsSub: Subscription;

    @ViewChild('stickyContainer', {static: true})
    public stickyContainer: Portal<any>;
    public stickyContainerOverlay?: OverlayRef;

    @ViewChildren(ItemCategoryDirective)
    public stickToTopElements: QueryList<ItemCategoryDirective>;

    public filter: {name?: string, dice?: number, category?: ItemCategory};
    public itemsByCategory: {[categoryId: number]: ItemTemplate[]} = {};
    public filteredCategories: ItemCategory[] = [];

    public stickyCategory: ItemCategory;
    public sticked?: ItemCategoryDirective;
    public expandedSticky = false;

    @HostListener('window:scroll', [])
    onWindowScroll() {
        if (this.inTab) {
            return;
        }
        let lastOutsideScreen: ItemCategoryDirective | undefined;
        for (let element of this.stickToTopElements.toArray()) {
            let top = element.elementRef.nativeElement.getBoundingClientRect().top;
            if (top > 20) {
                break;
            }
            lastOutsideScreen = element;
        }
        if (lastOutsideScreen && lastOutsideScreen !== this.sticked) {
            this.stickyCategory = lastOutsideScreen.category;
            this.openStickyContainer();
            this.sticked = lastOutsideScreen;
        }
        if (!lastOutsideScreen && this.sticked) {
            this.sticked = undefined;
            this.closeStickyContainer();
        }
    }

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

    openStickyContainer() {
        if (this.stickyContainerOverlay) {
            return;
        }
        let config = new OverlayConfig();
        config.width = '100%';
        config.positionStrategy = this.overlay.position()
            .global()
            .left('0')
            .top('0');

        this.stickyContainerOverlay = this.overlay.create(config);
        this.stickyContainerOverlay.attach(this.stickyContainer);
    }

    closeStickyContainer() {
        if (this.stickyContainerOverlay) {
            this.stickyContainerOverlay.detach();
            this.stickyContainerOverlay = undefined;
        }
    }

    expandStickyCategory() {
        this.expandedSticky = true;
    }

    collapseStickyCategory() {
        this.expandedSticky = false;
    }

    scrollToCategory(category: ItemCategory) {
        for (let element of this.stickToTopElements.toArray()) {
            if (element.category.id === category.id) {
                window.scrollBy({
                    top: element.elementRef.nativeElement.getBoundingClientRect().top,
                    behavior: 'smooth'
                });
            }
        }
        this.expandedSticky = false;
        return false;
    }

    backToTop() {
        smoothScrollTo(0, 0, 300);
    }

    resetFilter() {
        this.filter = {name: undefined, dice: undefined, category: undefined};
        this.updateFilter();
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
            this.filter.category = undefined;
            this.resetFilter();
            this.loadSection(section);
        }
        return false;
    }

    selectCategory(category: ItemCategory) {
        this.filter.category = category;
        this.updateFilter();
    }

    trackById(index, element) {
        return element.id;
    }

    isVisible(item: ItemTemplate) {
        if (this.filter.category) {
            if (item.categoryId !== this.filter.category.id) {
                return false;
            }
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
            this.updateFilter();
        });
    }

    updateFilter() {
        let filteredCategories: ItemCategory[] = [];
        let itemsByCategory: {[categoryId: number]: ItemTemplate[]} = {};
        for (let i = 0; i < this.items.length; i++) {
            let item = this.items[i];
            if (!this.isVisible(item)) {
                continue;
            }
            if (!itemsByCategory[item.categoryId]) {
                itemsByCategory[item.categoryId] = [];
                let category = this.selectedSection.categories.find(c => c.id === item.categoryId);
                if (category) {
                    filteredCategories.push(category);
                }
            }
            itemsByCategory[item.categoryId].push(item);
        }
        this.itemsByCategory = itemsByCategory;
        this.filteredCategories = filteredCategories;
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
        this.filter.category = itemCategory;
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
        ]).subscribe(([jobsName, originsName, sections, godsByTechName]: [{[jobId: number]: string}, {[jobId: number]: string}, ItemSection[], {[techName: string]: God}]) => {
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
        this.closeStickyContainer();
    }
}

