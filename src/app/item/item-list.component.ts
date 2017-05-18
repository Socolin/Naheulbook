import {Component, OnInit, OnDestroy, Input} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Subscription} from 'rxjs/Rx';

import {LoginService} from '../user/login.service';
import {OriginService} from '../origin/origin.service';
import {JobService} from '../job/job.service';

import {ItemSection, ItemTemplate} from './item-template.model';
import {ItemService} from './item.service';
import {removeDiacritics} from '../shared';

@Component({
    selector: 'item-list',
    templateUrl: './item-list.component.html',
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
    @Input() inTab: boolean;
    public itemSections: ItemSection[];
    public items: ItemTemplate[] = [];
    public itemsByCategory: {[categoryId: number]: ItemTemplate[]} = {};
    public selectedSection: ItemSection;
    public filter: {name: string, dice: number};
    public originsName: {[originId: number]: string};
    public jobsName: {[jobId: number]: string};
    public editable: boolean;
    public sub: Subscription;

    constructor(private _router: Router
        , private _route: ActivatedRoute
        , private _loginService: LoginService
        , private _itemService: ItemService
        , private _originService: OriginService
        , private _jobService: JobService) {
        this.resetFilter();
    }

    resetFilter() {
        this.filter = {name: null, dice: null};
    }

    selectSection(event: MouseEvent, section: ItemSection) {
        if (this.selectedSection && this.selectedSection.id === section.id) {
            if (event.ctrlKey) {
                this._itemService.clearItemSectionCache(section.id);
                this.loadSection(section);
            }
        } else {
            if (!this.inTab) {
                this._router.navigate(['../items'], {queryParams: {id: section.id}, relativeTo: this._route});
            }
            else {
                this.selectSectionById(section.id);
            }
        }
        return false;
    }

    isVisible(item) {
        if (item.data.diceDrop && this.filter && this.filter.dice) {
            return item.data.diceDrop === this.filter.dice;
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
        this.selectedSection = section;
        this._itemService.getItems(section).subscribe(items => {
            let itemsBySection: {[categoryId: number]: ItemTemplate[]} = {};
            for (let i = 0; i < items.length; i++) {
                let item = items[i];
                if (!itemsBySection[item.category]) {
                    itemsBySection[item.category] = [];
                }
                itemsBySection[item.category].push(item);
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
                this.resetFilter();
                this.loadSection(section);
                break;
            }
        }
    }

    ngOnInit() {
        this._itemService.getSectionsList().subscribe(sections => {
            this.itemSections = sections;
            this.sub = this._router.routerState.root.queryParams.subscribe(params => {
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

