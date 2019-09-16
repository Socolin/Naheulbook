import {Component, OnInit} from '@angular/core';
import {forkJoin} from 'rxjs';
import {MatDialog, MatDialogRef} from '@angular/material/dialog';
import {ItemTemplate, ItemTemplateService} from '../item-template';
import {IconDescription} from '../shared/icon.model';
import {ItemTemplateDialogComponent} from '../item-template/item-template-dialog.component';
import {MonsterSimpleInventory} from './monster.model';

@Component({
    templateUrl: './add-monster-item-dialog.component.html',
    styleUrls: ['./add-monster-item-dialog.component.scss']
})
export class AddMonsterItemDialogComponent implements OnInit {
    public filteredItemTemplates?: {
        name: string,
        icon: IconDescription,
        categoryName: string,
        sectionName: string,
        sourceIcon?: string,
        itemTemplate: ItemTemplate
    }[];
    public inventoryElement?: MonsterSimpleInventory;

    constructor(
        private dialog: MatDialog,
        private itemTemplateService: ItemTemplateService,
        public dialogRef: MatDialogRef<AddMonsterItemDialogComponent, MonsterSimpleInventory>
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

    selectItemTemplate(itemTemplate: ItemTemplate) {
        this.inventoryElement = {
            chance: 1,
            minCount: 1,
            maxCount: 1,
            hidden: false,
            itemTemplate,
            id: 0
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
            panelClass: 'app-dialog-no-padding',
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
