import {Component, Input, OnInit, OnChanges} from '@angular/core';
import {Router} from '@angular/router';
import {ItemTemplate, ItemSection, ItemCategory} from "./item-template.model";

import {OriginService} from "../origin";
import {JobService} from "../job";
import {Item} from "../character";


@Component({
    moduleId: module.id,
    selector: 'item-element',
    templateUrl: 'item-element.component.html',
    styleUrls: ['item-element.component.css']
})
export class ItemElementComponent implements OnInit, OnChanges {
    @Input() items: ItemTemplate[];
    @Input() restrictCategory: ItemCategory;
    @Input() type: ItemSection;
    @Input() editable: boolean;
    @Input() filter: {dice: number};

    public itemsByCategory: Object;
    public originsName: {[originId: number]: string};
    public jobsName: {[jobId: number]: string};
    public headers: string[];
    public columnsCount: number;

    constructor(private router: Router, private originService: OriginService, private jobService: JobService) {
        this.filter = {dice: null};
    }

    editItem(item: Item) {
        this.router.navigate(['/EditItem', {id: item.id}]);
    }

    ngOnChanges() {
        let headers: string[] = [];
        if (this.editable) {
            headers.push("E");
        }
        headers.push("Nom");
        headers.push("Prix(PO)");
        if (this.hasSpecial('HAVE_CHARGE')) {
            headers.push("Charges");
        }
        if (this.hasSpecial('HAVE_DAMAGE')) {
            headers.push("Degat");
        }
        if (this.hasSpecial('HAVE_PROTECTION')) {
            headers.push("PR");
        }
        if (this.hasSpecial('CAN_HAVE_MODIFIER')) {
            headers.push("Malus");
        }
        if (this.hasSpecial('CAN_HAVE_MODIFIER')) {
            headers.push("Bonus");
        }
        if (this.hasSpecial('CAN_HAVE_RUPTURE')) {
            headers.push("Rupture");
        }
        if (this.hasSpecial('DICE_DROP')) {
            headers.push("D100");
        }
        if (this.hasSpecial('HAVE_NOTE')) {
            headers.push("Note");
        }
        this.headers = headers;

        let itemsByCategory = {};
        for (let i = 0; i < this.items.length; i++) {
            let item = this.items[i];
            if (!itemsByCategory[item.category.id]) {
                itemsByCategory[item.category.id] = [];
            }
            itemsByCategory[item.category.id].push(item);
        }
        this.itemsByCategory = itemsByCategory;
    }

    isHidden(item: ItemTemplate) {
        if (this.hasSpecial('DICE_DROP')) {
            if (this.filter.dice) {
                return item.data.diceDrop !== this.filter.dice;
            }
        }
        return false;
    }

    hasSpecial(token: string) {
        if (this.type) {
            if (this.type.special.indexOf(token) !== -1) {
                return true;
            }
        }
        return false;
    }

    ngOnInit() {
        this.jobService.getJobList().subscribe(jobs => {
            let jobsName: {[jobId: number]: string} = {};
            for (let i = 0; i < jobs.length; i++) {
                let job = jobs[i];
                jobsName[job.id] = job.name;
            }
            this.jobsName = jobsName;
        });
        this.originService.getOriginList().subscribe(origins => {
            let originsName: {[originId: number]: string} = {};
            for (let i = 0; i < origins.length; i++) {
                let origin = origins[i];
                originsName[origin.id] = origin.name;
            }
            this.originsName = originsName;
        });
    }
}
