import {
    Component, OnInit, OnDestroy, Input, ViewChildren, HostListener, QueryList, ViewChild
} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Subscription} from 'rxjs/Rx';

import {LoginService} from '../user/login.service';
import {OriginService} from '../origin/origin.service';
import {JobService} from '../job/job.service';

import {ItemCategory, ItemSection, ItemTemplate} from './item-template.model';
import {ItemService} from './item.service';
import {removeDiacritics} from '../shared';
import {Observable} from 'rxjs/Observable';
import {Overlay, OverlayRef, OverlayState, Portal} from '@angular/material';
import {ItemCategoryDirective} from './item-category.directive';

@Component({
    selector: 'item-list',
    templateUrl: './item-list.component.html',
    styleUrls: ['item-list.component.scss'],
})
export class ItemListComponent implements OnInit, OnDestroy {
    @Input() inTab: boolean;
    public itemSections: ItemSection[];
    public items: ItemTemplate[] = [];
    public selectedSection: ItemSection;
    public originsName: {[originId: number]: string};
    public jobsName: {[jobId: number]: string};

    public editable: boolean;
    public queryParamsSub: Subscription;

    @ViewChild('stickyContainer')
    public stickyContainer: Portal<any>;
    public stickyContainerOverlay: OverlayRef;

    @ViewChildren(ItemCategoryDirective)
    public stickToTopElements: QueryList<ItemCategoryDirective>;

    public filter: {name: string, dice: number, category: ItemCategory};
    public itemsByCategory: {[categoryId: number]: ItemTemplate[]} = {};
    public filteredCategories: ItemCategory[] = [];

    public stickyCategory: ItemCategory;
    public sticked: ItemCategoryDirective;
    public expandedSticky = false;

    @HostListener('window:scroll', [])
    onWindowScroll() {
        if (this.inTab) {
            return;
        }
        let lastOutsideScreen: ItemCategoryDirective;
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
        , private _route: ActivatedRoute
        , private _loginService: LoginService
        , private _itemService: ItemService
        , private _originService: OriginService
        , private _jobService: JobService) {
        this.resetFilter();
    }

    openStickyContainer() {
        if (this.stickyContainerOverlay) {
            return;
        }
        let config = new OverlayState();
        config.positionStrategy = this.overlay.position()
            .global()
            .left('0')
            .top('0')
            .width('100%');

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

    scrollToCategory(category: ItemCategory) {
        for (let element of this.stickToTopElements.toArray()) {
            if (element.category.id === category.id) {
                window.scrollBy(0, element.elementRef.nativeElement.getBoundingClientRect().top);
            }
        }
        this.expandedSticky = false;
        return false;
    }

    resetFilter() {
        this.filter = {name: null, dice: null, category: null};
    }

    selectSectionById(sectionId: number) {
        if (this.selectedSection && this.selectedSection.id === sectionId) {
            return;
        }
        let i = this.itemSections.findIndex(s => s.id === sectionId);
        if (i !== -1) {
            this.selectSection(this.itemSections[i]);
        }
    }

    selectSection(section: ItemSection) {
        if (this.selectedSection && this.selectedSection.id === section.id) {
            this._itemService.clearItemSectionCache(section.id);
            this.loadSection(section);
        } else {
            if (!this.inTab) {
                this._router.navigate(['../items'], {queryParams: {id: section.id}, relativeTo: this._route});
            }

            this.selectedSection = section;
            this.filter.category = null;
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
            if (item.category !== this.filter.category.id) {
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
        this._itemService.getItems(section).subscribe(items => {
            this.items = items;
            this.updateFilter();
        });
    }

    updateFilter() {
        let filteredCategories = [];
        let itemsByCategory: {[categoryId: number]: ItemTemplate[]} = {};
        for (let i = 0; i < this.items.length; i++) {
            let item = this.items[i];
            if (!this.isVisible(item)) {
                continue;
            }
            if (!itemsByCategory[item.category]) {
                itemsByCategory[item.category] = [];
                let category = this.selectedSection.categories.find(c => c.id === item.category);
                filteredCategories.push(category);
            }
            itemsByCategory[item.category].push(item);
        }
        this.itemsByCategory = itemsByCategory;
        this.filteredCategories = filteredCategories;
    }

    hasSpecial(token: string) {
        if (this.selectedSection) {
            if (this.selectedSection.special.indexOf(token) !== -1) {
                return true;
            }
        }
        return false;
    }

    ngOnInit() {
        Observable.forkJoin(
            this._jobService.getJobsNamesById(),
            this._originService.getOriginsNamesById(),
            this._itemService.getSectionsList()
        ).subscribe(([jobsName, originsName, sections]: [{[jobId: number]: string}, {[jobId: number]: string}, ItemSection[]]) => {
            this.originsName = originsName;
            this.jobsName = jobsName;
            this.itemSections = sections;
            if (!this._route.snapshot.data['id']) {
                this.selectSection(sections[0]);
            }
            this.queryParamsSub = this._route.queryParams.subscribe(params => {
                this.selectSectionById(+params['id']);
            });
        });

        this._loginService.loggedUser.subscribe(
            user => {
                this.editable = user && user.admin;
            });
    }

    ngOnDestroy() {
        if (this.queryParamsSub) {
            this.queryParamsSub.unsubscribe();
        }
    }
}

