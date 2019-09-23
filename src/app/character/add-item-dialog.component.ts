import {Component, ViewChild} from '@angular/core';
import {ItemTemplate, ItemTemplateService} from '../item-template';
import {ItemData} from '../item';
import {MatDialogRef, MatStep} from '@angular/material';
import {forkJoin} from 'rxjs';
import {IconDescription} from '../shared/icon.model';

export interface AddItemDialogResult {
    itemTemplateId: number;
    itemData: ItemData;
}

@Component({
    selector: 'app-add-item-dialog',
    templateUrl: './add-item-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './add-item-dialog.component.scss']
})
export class AddItemDialogComponent {
    @ViewChild('searchStep', {static: true})
    public searchStep: MatStep;

    public selectedItemTemplate: ItemTemplate | undefined;
    public itemData?: ItemData;
    public filteredItemTemplates: {
        name: string,
        icon: IconDescription,
        categoryName: string,
        sectionName: string,
        sourceIcon?: string,
        itemTemplate: ItemTemplate
    }[];

    constructor(
        private itemTemplateService: ItemTemplateService,
        public dialogRef: MatDialogRef<AddItemDialogComponent, AddItemDialogResult>,
    ) {
    }

    updateFilter(filter: string): void {
        forkJoin([
            this.itemTemplateService.getCategoriesById(),
            this.itemTemplateService.searchItem(filter)
        ]).subscribe(([categoriesById, itemTemplates]) => {
            this.filteredItemTemplates = itemTemplates.map(itemTemplate => {
                let name = itemTemplate.name;
                if (itemTemplate.data.enchantment !== undefined) {
                    name += ' (Ench. ' + itemTemplate.data.enchantment + ')';
                }
                let sourceIcon;
                if (itemTemplate.source === 'community') {
                    sourceIcon = 'group';
                } else if (itemTemplate.source === 'private') {
                    sourceIcon = 'lock';
                }
                return {
                    name,
                    icon: itemTemplate.data.icon,
                    sectionName: categoriesById[itemTemplate.categoryId].section.name,
                    categoryName: categoriesById[itemTemplate.categoryId].name,
                    sourceIcon,
                    itemTemplate,
                };
            });
        });
    }

    selectItemTemplate(itemTemplate: ItemTemplate): void {
        this.selectedItemTemplate = itemTemplate;

        this.itemData = new ItemData();
        this.itemData.name = itemTemplate.name;
        if (itemTemplate.data.quantifiable) {
            this.itemData.quantity = 1;
        } else {
            delete this.itemData.quantity;
        }
        this.searchStep.completed = true;
    }

    addItem() {
        if (!this.selectedItemTemplate) {
            return;
        }
        if (!this.itemData) {
            return;
        }
        this.dialogRef.close({
            itemTemplateId: this.selectedItemTemplate.id,
            itemData: this.itemData,
        });
    }
}
