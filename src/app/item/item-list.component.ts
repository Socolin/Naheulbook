import {Component} from '@angular/core';
import {Router} from '@angular/router';

import {LoginService} from "../user";
import {OriginService} from "../origin";
import {JobService} from "../job";

import {ItemElementComponent} from './item-element.component';
import {ItemSection, ItemTemplate} from "./item-template.model";
import {ItemService} from "./item.service";
import {removeDiacritics, ModifierPipe} from "../shared";

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
export class ItemListComponent {
    public itemSections: ItemSection[];
    public items: ItemTemplate[] = [];
    public itemsByCategory: {[categoryId: number]: ItemTemplate[]} = {};
    public selectedSection: ItemSection;
    public filter: {name: string, dice: number};
    public originsName: Object;
    public jobsName: Object;
    public editable: boolean = true;

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
            return item.diceDrop == this.filter.dice;
        }
        if (this.filter && this.filter.name) {
            var cleanFilter = removeDiacritics(this.filter.name).toLowerCase();
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
            if (this.selectedSection.special.indexOf(token) != -1) {
                return true;
            }
        }
        return false;
    }

    selectSectionById(sectionId: number) {
        if (this.selectedSection && this.selectedSection.id == sectionId) {
            return;
        }

        for (let i = 0; i < this.itemSections.length; i++) {
            var section = this.itemSections[i];
            if (section.id == sectionId) {
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
            this._router.routerState.queryParams.subscribe(params => {
                let id = +params['id'];
                this.selectSectionById(id);
            });
        });

        this._jobService.getJobList().subscribe(jobs => {
            let jobsName = {};
            for (let i = 0; i < jobs.length; i++) {
                let job = jobs[i];
                jobsName[job.id] = job.name;
            }
            this.jobsName = jobsName;
        });

        this._originService.getOriginList().subscribe(origins => {
            let originsName = {};
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
}

