import {Component, OnInit, OnDestroy} from '@angular/core';
import {Router} from '@angular/router';

import {LoginService} from "../user";
import {OriginService} from "../origin";
import {JobService} from "../job";

import {ItemElementComponent} from './item-element.component';
import {ItemSection, ItemTemplate} from "./item-template.model";
import {ItemService} from "./item.service";
import {removeDiacritics, ModifierPipe} from "../shared";
import {Subscription} from 'rxjs/Rx';

@Component({
    templateUrl: 'app/item/item-list.component.html',
    pipes: [ModifierPipe],
    directives: [ItemElementComponent],
    styles: [
        `.item-section {
            font-weight: bold;
        }
        .item-element:nth-child(2n) {
            background: #eeeeee;
        }
        `
    ]
})
export class ItemListComponent implements OnInit, OnDestroy {
    private itemSections: ItemSection[];
    private items: ItemTemplate[] = [];
    private itemsByCategory: {[categoryId: number]: ItemTemplate[]} = {};
    private selectedSection: ItemSection;
    private filter: {name: string, dice: number};
    private originsName: {[originId: number]: string};
    private jobsName: {[jobId: number]: string};
    private editable: boolean = true;
    private sub: Subscription;

    constructor(private _router: Router
        , private _loginService: LoginService
        , private _itemService: ItemService
        , private _originService: OriginService
        , private _jobService: JobService) {
        this.resetFilter();
    }

    resetFilter() {
        this.filter = {name: null, dice: null};
    }

    selectSection(section: ItemSection) {
        this._router.navigate(['/database/items'], {queryParams: {id: section.id}});
        return false;
    }

    isVisible(item) {
        if (item.diceDrop && this.filter && this.filter.dice) {
            return item.diceDrop === this.filter.dice;
        }
        if (this.filter && this.filter.name) {
            let cleanFilter = removeDiacritics(this.filter.name).toLowerCase();
            return removeDiacritics(item.name).toLowerCase().indexOf(cleanFilter) > -1;
        }
        return true;
    }

    isItemVisibleInList(items: ItemTemplate[]) {
        if (!items) {
            return;
        }
        for (let i = 0; i < items.length; i++) {
            let item = items[i];
            if (this.isVisible(item)) {
                return true;
            }
        }
        return false;
    }

    loadSection(section: ItemSection) {
        this.resetFilter();
        this.selectedSection = section;
        this._itemService.getItems(section).subscribe(items => {
            let itemsBySection: {[categoryId: number]: ItemTemplate[]} = {};
            for (let i = 0; i < items.length; i++) {
                let item = items[i];
                if (!itemsBySection[item.category.id]) {
                    itemsBySection[item.category.id] = [];
                }
                itemsBySection[item.category.id].push(item);
            }
            this.items = items;
            this.itemsByCategory = itemsBySection;
        });

    }

    hasSpecial(token: string) {
        if (this.selectedSection) {
            if (this.selectedSection.special.indexOf(token) !== -1) {
                return true;
            }
        }
        return false;
    }

    selectSectionById(sectionId: number) {
        if (this.selectedSection && this.selectedSection.id === sectionId) {
            return;
        }

        for (let i = 0; i < this.itemSections.length; i++) {
            let section = this.itemSections[i];
            if (section.id === sectionId) {
                this.loadSection(section);
                break;
            }
        }
    }

    editItem(item: ItemTemplate) {
        this._router.navigate(['/edit-item', item.id]);
    }

    ngOnInit() {
        this._itemService.getSectionsList().subscribe(sections => {
            this.itemSections = sections;
            this.sub = this._router.routerState.queryParams.subscribe(params => {
                let id = +params['id'];
                this.selectSectionById(id);
            });
        });

        this._jobService.getJobList().subscribe(jobs => {
            let jobsName: {[jobId: number]: string} = {};
            for (let i = 0; i < jobs.length; i++) {
                let job = jobs[i];
                jobsName[job.id] = job.name;
            }
            this.jobsName = jobsName;
        });

        this._originService.getOriginList().subscribe(origins => {
            let originsName: {[originId: number]: string} = {};
            for (let i = 0; i < origins.length; i++) {
                let origin = origins[i];
                originsName[origin.id] = origin.name;
            }
            this.originsName = originsName;
        });
        this._loginService.loggedUser.subscribe(
            user => {
                this.editable = user && user.admin;
            },
            err => {
                this.editable = false;
                console.log(err);
            });
    }

    ngOnDestroy() {
        if (this.sub) {
            this.sub.unsubscribe();
        }
    }
}

