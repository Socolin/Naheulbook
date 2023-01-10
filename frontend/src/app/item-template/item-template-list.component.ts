import {forkJoin, Observable, of, Subject, Subscription} from 'rxjs';
import {Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild} from '@angular/core';
import {animate, state, style, transition, trigger} from '@angular/animations';
import {Overlay} from '@angular/cdk/overlay';
import {ActivatedRoute, Router} from '@angular/router';

import {God, MiscService, removeDiacritics} from '../shared';

import {LoginService} from '../user';
import {OriginService} from '../origin';
import {JobService} from '../job';

import {ItemTemplate, ItemTemplateSection, ItemTemplateSubCategory} from './item-template.model';
import {ItemTemplateService} from './item-template.service';
import {CreateItemTemplateDialogComponent, CreateItemTemplateDialogData} from './create-item-template-dialog.component';
import {NhbkMatDialog} from '../material-workaround';
import {Guid} from '../api/shared/util';
import {EditItemTemplateDialogComponent, EditItemTemplateDialogData} from './edit-item-template-dialog.component';
import {BreakpointObserver, Breakpoints} from '@angular/cdk/layout';
import {UntypedFormBuilder, UntypedFormControl} from '@angular/forms';
import {debounceTime, map, startWith, switchMap, tap} from 'rxjs/operators';

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
    public searchItems?: ItemTemplate[];
    public selectedSection?: ItemTemplateSection;
    public originsName: { [originId: string]: string };
    public jobsName: { [jobId: string]: string };
    public godsByTechName: { [techName: string]: God };

    public queryParamsSub: Subscription;

    public visibleItems: ItemTemplate[] = [];
    public tableView: boolean;
    public showCommunityItems: boolean;

    public categoryNameControl = new UntypedFormControl();
    public filteredItemCategories: Observable<ItemTemplateSection[]>;

    public searching: boolean;
    public searchSubject: Subject<string> = new Subject<string>();

    @ViewChild('searchInput', {static: true})
    private searchInputElement: ElementRef<HTMLInputElement>;

    constructor(
        public readonly loginService: LoginService,
        private readonly dialog: NhbkMatDialog,
        private readonly formBuilder: UntypedFormBuilder,
        private readonly itemTemplateService: ItemTemplateService,
        private readonly jobService: JobService,
        private readonly miscService: MiscService,
        private readonly originService: OriginService,
        private readonly overlay: Overlay,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly breakpointObserver: BreakpointObserver,
    ) {
        this.searchSubject
            .pipe(
                debounceTime(200),
                tap(() => this.searching = true),
                switchMap(filter => {
                    if (!filter) {
                        return of<ItemTemplate[] | undefined>(undefined);
                    }
                    return this.itemTemplateService.searchItem(filter)
                })
            )
            .subscribe({
                next: items => {
                    if (items === undefined) {
                        this.searchItems = undefined;
                        this.updateVisibleItems();
                    }
                    this.searchItems = items;
                    this.updateVisibleItems();
                    this.searching = false;
                }, error: () => {
                    this.searching = false;
                }
            })
    }

    updateViewCommunityItems(checked: boolean) {
        this.showCommunityItems = checked;
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
        if (item.source === 'community' && !this.showCommunityItems) {
            return false;
        }
        return true;
    }

    updateVisibleItems() {
        if (this.searchItems !== undefined) {
            this.visibleItems = this.searchItems
        } else {
            this.visibleItems = this.items.filter(item => this.isVisible(item));
        }
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

    selectSectionById(sectionId: number) {
        if (this.selectedSection && this.selectedSection.id === sectionId) {
            return;
        }
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

    selectSubCategory(itemSubCategory: ItemTemplateSubCategory, scrollToFirstItem = false) {
        if (!this.selectedSection) {
            return;
        }
        this.selectSection(itemSubCategory.section);
        this.selectedItemSubCategory = itemSubCategory;
        this.updateNextAndPreviousSubCategory();
        this.resetSearch();
        this.updateVisibleItems();

        const firstItem = document.getElementById('first-item');
        if (scrollToFirstItem && firstItem) {
            firstItem.scrollIntoView();
        }
        this.updateUrl();
    }

    private resetSearch() {
        this.searchItems = undefined;
        this.searchInputElement.nativeElement.value = '';
    }

    updateAutocompleteItem(filter: string) {
        this.searchSubject.next(filter);
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

    openEditItemTemplateDialog(itemTemplate: ItemTemplate) {
        const dialogRef = this.dialog.openFullScreen<EditItemTemplateDialogComponent, EditItemTemplateDialogData, ItemTemplate>(
            EditItemTemplateDialogComponent,
            {
                data: {itemTemplateId: itemTemplate.id}
            }
        );
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.reloadSectionForItem(itemTemplate);
            this.reloadSectionForItem(result);
        });
    }

    getSubCategoryName(subCategory?: ItemTemplateSubCategory) {
        return subCategory?.name;
    };

    private reloadCategory(category: ItemTemplateSection) {
        this.itemTemplateService.clearItemSectionCache(category.id);
        this.loadSection(category);
    }

    ngOnInit() {
        this.tableView = this.breakpointObserver.isMatched([Breakpoints.Medium, Breakpoints.Large, Breakpoints.XLarge])
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
            this.filteredItemCategories = this.categoryNameControl.valueChanges
                .pipe(
                    startWith(''),
                    map(value => this.filterItemCategories(value))
                )
        });
    }

    ngOnDestroy() {
        if (this.queryParamsSub) {
            this.queryParamsSub.unsubscribe();
        }
    }

    openCreateCopyItemTemplateDialog(sourceItem: ItemTemplate) {
        const dialogRef = this.dialog.openFullScreen<CreateItemTemplateDialogComponent, CreateItemTemplateDialogData, ItemTemplate>(
            CreateItemTemplateDialogComponent,
            {
                data: {copyFromItemTemplateId: sourceItem.id}
            }
        );
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.reloadSectionForItem(result);
        });
    }

    private filterItemCategories(value: string | ItemTemplateSubCategory): ItemTemplateSection[] {
        if (value) {
            let cleanedValue: string;
            if (typeof value === 'string') {
                cleanedValue = removeDiacritics(value.toLowerCase());
            } else {
                cleanedValue = removeDiacritics(value.name.toLowerCase());
            }

            return this.itemSections
                .map(category => category.cloneFilterSubCategories(
                    (s => removeDiacritics(s.name.toLowerCase()).indexOf(cleanedValue) !== -1
                        || removeDiacritics(s.section.name.toLowerCase()).indexOf(cleanedValue) !== -1))
                )
                .filter(group => group.subCategories.length > 0);

        }
        return this.itemSections;
    }
}

