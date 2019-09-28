import {Component, Inject, OnInit} from '@angular/core';
import {forkJoin, Observer, Subject} from 'rxjs';
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from '@angular/material/dialog';
import {Item} from '../item';
import {ItemTemplate, ItemTemplateService} from '../item-template';
import {IconDescription} from '../shared/icon.model';
import {ItemTemplateDialogComponent} from '../item-template/item-template-dialog.component';
import {IconSelectorComponent, IconSelectorComponentDialogData} from '../shared';

export interface CreateItemDialogDialog {
    onAdd: Observer<any>;
    allowMultipleAdd: boolean,
    itemTemplate: ItemTemplate
}

@Component({
    selector: 'app-create-item-dialog',
    templateUrl: './create-item-dialog.component.html',
    styleUrls: ['./create-item-dialog.component.scss']
})
export class CreateItemDialogComponent implements OnInit {
    public newItem: Item = new Item();
    public filteredItemTemplates?: {
        name: string,
        icon: IconDescription,
        categoryName: string,
        sectionName: string,
        sourceIcon?: string,
        itemTemplate: ItemTemplate
    }[];

    constructor(
        private dialog: MatDialog,
        private itemTemplateService: ItemTemplateService,
        public dialogRef: MatDialogRef<CreateItemDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: CreateItemDialogDialog
    ) {
        if (data.itemTemplate) {
            this.selectItemTemplate(data.itemTemplate);
        }
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

    setItemNotIdentified() {
        this.newItem.data.notIdentified = true;
        if (this.newItem.template && this.newItem.template.data && this.newItem.template.data.notIdentifiedName) {
            this.newItem.data.name = this.newItem.template.data.notIdentifiedName;
        }
    }

    setItemIdentified() {
        delete this.newItem.data.notIdentified;
        if (this.newItem.template && this.newItem.template.name) {
            this.newItem.data.name = this.newItem.template.name;
        }
    }

    openSelectIconDialog() {
        const dialogRef = this.dialog.open<IconSelectorComponent, IconSelectorComponentDialogData, IconDescription>(IconSelectorComponent, {
            data: {icon: this.newItem.data.icon}
        });

        dialogRef.afterClosed().subscribe((icon) => {
            if (!icon) {
                return;
            }
            this.newItem.data.icon = icon;
        })
    }

    updateItemIdentified() {
        if (this.newItem.data.notIdentified) {
            this.setItemNotIdentified();
        } else {
            this.setItemIdentified();
        }
    }

    ngOnInit() {
    }

    selectItemTemplate(itemTemplate: ItemTemplate) {
        this.newItem.template = itemTemplate;
        if (!this.newItem.data.notIdentified) {
            this.newItem.data.name = itemTemplate.name;
        } else {
            if (this.newItem.template && this.newItem.template.data && this.newItem.template.data.notIdentifiedName) {
                this.newItem.data.name = this.newItem.template.data.notIdentifiedName;
            }
        }
        if (itemTemplate.data.icon) {
            this.newItem.data.icon = JSON.parse(JSON.stringify(itemTemplate.data.icon));
        } else {
            this.newItem.data.icon = undefined;
        }
        this.newItem.data.description = itemTemplate.data.description;
        if (itemTemplate.data.quantifiable) {
            this.newItem.data.quantity = 1;
        } else {
            delete this.newItem.data.quantity;
        }
        if (itemTemplate.data.useUG) {
            this.newItem.data.ug = 1;
        } else {
            delete this.newItem.data.ug;
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

    addItem(close: boolean) {
        this.data.onAdd.next(this.newItem);
        if (close) {
            this.dialogRef.close();
        }
    }
}

export function openCreateItemDialog(dialog: MatDialog, onAdd: (item: Item) => void, allowMultipleAdd = true, itemTemplate?: ItemTemplate) {
    const subject = new Subject<Item>();
    const dialogRef = dialog.open(CreateItemDialogComponent, {
        data: {
            onAdd: subject,
            allowMultipleAdd,
            itemTemplate
        }
    });
    const subscription = subject.subscribe((item) => {
        onAdd(item);
    });

    dialogRef.afterClosed().subscribe((result) => {
        subscription.unsubscribe();
    });
}
