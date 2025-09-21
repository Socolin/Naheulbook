import {Component, ViewChild} from '@angular/core';
import {ItemTemplate, ItemTemplateService} from '../item-template';
import {ItemData} from '../item';
import { MatDialogRef, MatDialogClose } from '@angular/material/dialog';
import { MatStep, MatStepper, MatStepLabel } from '@angular/material/stepper';
import {forkJoin} from 'rxjs';
import {IconDescription} from '../shared/icon.model';
import {Guid} from '../api/shared/util';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatActionList, MatListItem } from '@angular/material/list';
import { IconComponent } from '../shared/icon.component';
import { FormsModule } from '@angular/forms';

export interface AddItemDialogResult {
    itemTemplateId: Guid;
    itemData: ItemData;
}

@Component({
    selector: 'app-add-item-dialog',
    templateUrl: './add-item-dialog.component.html',
    styleUrls: ['../shared/full-screen-dialog.scss', './add-item-dialog.component.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon, MatStepper, MatStep, MatStepLabel, MatFormField, MatInput, MatActionList, MatListItem, IconComponent, FormsModule, MatButton]
})
export class AddItemDialogComponent {
    @ViewChild('searchStep', {static: true})
    public searchStep: MatStep;

    public selectedItemTemplate: ItemTemplate | undefined;
    public itemData?: ItemData;
    public filteredItemTemplates: {
        name: string,
        icon?: IconDescription,
        subCategoryName: string,
        sectionName: string,
        sourceIcon?: string,
        itemTemplate: ItemTemplate
    }[];

    constructor(
        private readonly dialogRef: MatDialogRef<AddItemDialogComponent, AddItemDialogResult>,
        private readonly itemTemplateService: ItemTemplateService,
    ) {
    }

    updateFilter(filter: string): void {
        forkJoin([
            this.itemTemplateService.getSubCategoriesById(),
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
                    sectionName: categoriesById[itemTemplate.subCategoryId].section.name,
                    subCategoryName: categoriesById[itemTemplate.subCategoryId].name,
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
        if (itemTemplate.data.useUG) {
            this.itemData.ug = 1;
        } else {
            delete this.itemData.ug;
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
