import {Component, Inject, OnInit} from '@angular/core';
import {forkJoin} from 'rxjs';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import {ItemTemplate, ItemTemplateService} from '../item-template';
import {IconDescription} from '../shared/icon.model';
import {ItemTemplateDialogComponent} from '../item-template/item-template-dialog.component';
import {MonsterInventoryElement} from './monster.model';
import {NhbkMatDialog} from '../material-workaround';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatCardContent } from '@angular/material/card';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatActionList, MatListItem } from '@angular/material/list';
import { IconComponent } from '../shared/icon.component';
import { MatRipple } from '@angular/material/core';
import { FormsModule } from '@angular/forms';
import { MatCheckbox } from '@angular/material/checkbox';

export interface AddMonsterItemDialogData {
    inventoryElement?: MonsterInventoryElement;
}

@Component({
    templateUrl: './add-monster-item-dialog.component.html',
    styleUrls: ['./add-monster-item-dialog.component.scss'],
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, MatCardContent, MatFormField, MatInput, MatIconButton, MatIcon, MatActionList, MatListItem, IconComponent, MatRipple, FormsModule, MatCheckbox, MatDialogActions, MatButton, MatDialogClose]
})
export class AddMonsterItemDialogComponent implements OnInit {
    public filteredItemTemplates?: {
        name: string,
        icon?: IconDescription,
        subCategoryName: string,
        sectionName: string,
        sourceIcon?: string,
        itemTemplate: ItemTemplate
    }[];
    public inventoryElement?: MonsterInventoryElement;

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly itemTemplateService: ItemTemplateService,
        private readonly dialogRef: MatDialogRef<AddMonsterItemDialogComponent, MonsterInventoryElement>,
        @Inject(MAT_DIALOG_DATA) public readonly data: AddMonsterItemDialogData,
    ) {
        if (data.inventoryElement) {
            this.inventoryElement = {...data.inventoryElement};
        }
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

    selectItemTemplate(itemTemplate: ItemTemplate) {
        this.inventoryElement = {
            id: 0,
            chance: 1,
            minCount: 1,
            maxCount: 1,
            hidden: false,
            itemTemplate
        };
        if (itemTemplate.data.useUG) {
            this.inventoryElement.minUg = 1;
            this.inventoryElement.maxUg = 1;
        }
        this.filteredItemTemplates = undefined;
    }

    showInfo($event: MouseEvent, itemTemplate: ItemTemplate) {
        $event.stopImmediatePropagation();
        $event.stopPropagation();
        $event.preventDefault();

        this.dialog.open(ItemTemplateDialogComponent, {
            data: {itemTemplate},
            autoFocus: false
        });
    }

    addItem() {
        this.dialogRef.close(this.inventoryElement);
    }

    ngOnInit() {
    }
}
