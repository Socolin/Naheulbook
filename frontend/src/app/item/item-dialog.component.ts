import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {Item, ItemData} from './item.model';
import {forkJoin} from 'rxjs';
import {ItemTemplateSubCategory, ItemTemplateService} from '../item-template';
import {God, IconSelectorComponent, IconSelectorComponentDialogData, MiscService} from '../shared';
import {JobService} from '../job';
import {OriginService} from '../origin';
import {ItemService} from './item.service';
import {IconDescription} from '../shared/icon.model';
import {NhbkMatDialog} from '../material-workaround';

export interface ItemDialogData {
    readonly item: Item;
}

@Component({
    selector: 'app-item-dialog',
    templateUrl: './item-dialog.component.html',
    styleUrls: ['./item-dialog.component.scss'],
    standalone: false
})
export class ItemDialogComponent implements OnInit {
    public jobsName?: { [id: number]: string };
    public originsName?: { [id: number]: string };
    public itemCategoriesById: { [subCategoryId: number]: ItemTemplateSubCategory };
    public godsByTechName: { [name: string]: God };

    public get item(): Item {
        return this.data.item;
    }

    constructor(
        private readonly jobService: JobService,
        private readonly originService: OriginService,
        private readonly itemTemplateService: ItemTemplateService,
        private readonly miscService: MiscService,
        private readonly itemService: ItemService,
        private readonly dialog: NhbkMatDialog,
        @Inject(MAT_DIALOG_DATA) public readonly data: ItemDialogData
    ) {
    }

    updateItemQuantity(newQuantity: number) {
        this.itemService.updateItem(this.data.item.id, {
            ...this.data.item.data,
            quantity: newQuantity
        }).subscribe((item) => {
            this.data.item.data = {...item.data}
        });
    }

    openSelectIconDialog() {
        const dialogRef = this.dialog.open<IconSelectorComponent, IconSelectorComponentDialogData, IconDescription>(
            IconSelectorComponent, {
                data: {icon: this.data.item.data.icon}
            }
        );

        dialogRef.afterClosed().subscribe((icon) => {
            if (!icon) {
                return;
            }
            this.itemService.updateItem(this.data.item.id, {
                ...this.data.item.data,
                icon: icon
            }).subscribe((item) => {
                this.data.item.data = {...item.data}
            });
        })
    }

    ngOnInit() {
        forkJoin([
            this.jobService.getJobList(),
            this.originService.getOriginList(),
            this.itemTemplateService.getSubCategoriesById(),
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
