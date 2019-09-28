import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {Item} from './item.model';
import {forkJoin} from 'rxjs';
import {ItemTemplateCategory, ItemTemplateService} from '../item-template';
import {God, MiscService} from '../shared';
import {JobService} from '../job';
import {OriginService} from '../origin';

export interface ItemDialogData {
    item: Item;
}

@Component({
    selector: 'app-item-dialog',
    templateUrl: './item-dialog.component.html',
    styleUrls: ['./item-dialog.component.scss']
})
export class ItemDialogComponent implements OnInit {
    public jobsName?: { [id: number]: string };
    public originsName?: { [id: number]: string };
    public itemCategoriesById: { [categoryId: number]: ItemTemplateCategory };
    public godsByTechName: { [name: string]: God };

    public get item(): Item {
        return this.data.item;
    }

    constructor(
        private jobService: JobService,
        private originService: OriginService,
        private itemTemplateService: ItemTemplateService,
        private miscService: MiscService,
        @Inject(MAT_DIALOG_DATA) public data: ItemDialogData
    ) {
    }

    ngOnInit() {
        forkJoin([
            this.jobService.getJobList(),
            this.originService.getOriginList(),
            this.itemTemplateService.getCategoriesById(),
            this.miscService.getGodsByTechName(),
        ]).subscribe(
            ([jobs, origins, categoriesById, godsByTechName]) => {
                this.jobsName = jobs.reduce((result, job) => {
                    result[job.id] = job.name;
                    return result;
                }, {});
                this.originsName = origins.reduce((result, origin) => {
                    result[origin.id] = origin.name;
                    return result;
                }, {});
                this.godsByTechName = godsByTechName;
                this.itemCategoriesById = categoriesById;
            }
        );
    }
}
