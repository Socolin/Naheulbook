import {Component, Inject, OnInit} from '@angular/core';
import {forkJoin, Observer, Subject} from 'rxjs';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import {Item} from '../item';
import {ItemTemplate, ItemTemplateService} from '../item-template';
import {IconDescription} from '../shared/icon.model';
import {ItemTemplateDialogComponent} from '../item-template/item-template-dialog.component';
import {IconSelectorComponent, IconSelectorComponentDialogData} from '../shared';
import {NhbkMatDialog} from '../material-workaround';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatCardContent } from '@angular/material/card';
import { MatFormField, MatHint } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatActionList, MatListItem } from '@angular/material/list';
import { IconComponent } from '../shared/icon.component';
import { MatRipple } from '@angular/material/core';
import { FormsModule } from '@angular/forms';
import { MatCheckbox } from '@angular/material/checkbox';
import { ItemPricePipe } from '../item-template/item-price.pipe';

export interface CreateItemDialogDialog {
    onAdd: Observer<any>;
    allowMultipleAdd: boolean,
    itemTemplate: ItemTemplate
}

@Component({
    selector: 'app-create-item-dialog',
    templateUrl: './create-item-dialog.component.html',
    styleUrls: ['./create-item-dialog.component.scss'],
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, MatCardContent, MatFormField, MatInput, MatIconButton, MatIcon, MatActionList, MatListItem, IconComponent, MatRipple, FormsModule, MatHint, MatCheckbox, MatDialogActions, MatButton, MatDialogClose, ItemPricePipe]
})
export class CreateItemDialogComponent implements OnInit {
    public newItem: Item = new Item();
    public filteredItemTemplates?: {
        name: string,
        icon?: IconDescription,
        subCategoryName: string,
        sectionName: string,
        sourceIcon?: string,
        itemTemplate: ItemTemplate
    }[];

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly itemTemplateService: ItemTemplateService,
        private readonly dialogRef: MatDialogRef<CreateItemDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public readonly data: CreateItemDialogDialog
    ) {
        if (data.itemTemplate) {
            this.selectItemTemplate(data.itemTemplate);
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

export function openCreateItemDialog(
    dialog: NhbkMatDialog,
    onAdd: (item: Item) => void,
    allowMultipleAdd = true,
    itemTemplate?: ItemTemplate,
    closeOnNavigation?: boolean
) {
    const subject = new Subject<Item>();
    const dialogRef = dialog.open(CreateItemDialogComponent, {
        closeOnNavigation: closeOnNavigation,
        data: {
            onAdd: subject,
            allowMultipleAdd,
            itemTemplate
        }
    });
    const subscription = subject.subscribe((item) => {
        onAdd(item);
    });

    dialogRef.afterClosed().subscribe(() => {
        subscription.unsubscribe();
    });
}
