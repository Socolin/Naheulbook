import {Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, ViewChild} from '@angular/core';
import {Item, ItemData} from './item.model';
import {MatCheckbox} from '@angular/material/checkbox';
import {IconDescription} from '../shared/icon.model';
import {
    IconSelectorComponent,
    IconSelectorComponentDialogData,
    PromptDialogComponent,
    PromptDialogData,
    PromptDialogResult
} from '../shared';
import {ItemService} from './item.service';
import {NhbkMatDialog} from '../material-workaround';
import {IItemData} from '../api/shared';

@Component({
    selector: 'app-item-list',
    templateUrl: './item-list.component.html',
    styleUrls: ['./item-list.component.scss']
})
export class ItemListComponent {
    @Input()
    public items: Item[];
    @Input()
    public itemsExistsOnServer = true;
    @Output()
    public selectItem: EventEmitter<Item> = new EventEmitter<Item>();
    @Output()
    public deleteItems: EventEmitter<Item[]> = new EventEmitter<Item[]>();

    @ViewChild('selectAllCheckbox', {static: false})
    private selectAllCheckbox: MatCheckbox;
    public selectedItemsByIds: { [itemId: number]: Item } = {};

    constructor(
        private readonly itemService: ItemService,
        private readonly dialog: NhbkMatDialog,
    ) {
    }

    toggleSelectAll(checked: boolean) {
        if (checked) {
            this.selectedItemsByIds = this.items.reduce((result, item) => {
                result[item.id] = item;
                return result
            }, {});
        } else {
            this.selectedItemsByIds = {};
        }
    }

    toggleSelectItem(item: Item, checked: boolean) {
        if (checked) {
            this.selectedItemsByIds[item.id] = item;
        } else {
            delete this.selectedItemsByIds[item.id];
        }

        const selectedItemsIds = Object.keys(this.selectedItemsByIds);
        if (selectedItemsIds.length === 0) {
            this.selectAllCheckbox.checked = false;
            this.selectAllCheckbox.indeterminate = false;
        } else if (selectedItemsIds.length === this.items.length) {
            this.selectAllCheckbox.checked = true;
            this.selectAllCheckbox.indeterminate = false;
        } else {
            this.selectAllCheckbox.checked = true;
            this.selectAllCheckbox.indeterminate = true;
        }
    }

    markItemsAsNotIdentified(items?: Item[]) {
        const selectedItems = items || Object.values(this.selectedItemsByIds);
        this.updateItemsData(selectedItems, (item => ({
            ...item.data,
            notIdentified: true,
            name: item.template.data.notIdentifiedName || item.template.name
        })));
    }

    identifyItems(items?: Item[]) {
        const selectedItems = items || Object.values(this.selectedItemsByIds);
        this.updateItemsData(selectedItems, item => {
            const newItemData = {
                ...item.data,
                name: item.data.name === item.template.data.notIdentifiedName ? item.template.name : item.data.name
            };
            delete newItemData.notIdentified;
            return newItemData;
        });
    }

    openSelectIconDialog(items?: Item[]): void {
        const selectedItems = items || Object.values(this.selectedItemsByIds);
        const dialogRef = this.dialog.open<IconSelectorComponent, IconSelectorComponentDialogData, IconDescription>(
            IconSelectorComponent, {
                data: {icon: selectedItems[0].data.icon}
            }
        );

        dialogRef.afterClosed().subscribe((icon) => {
            if (!icon) {
                return;
            }
            this.updateItemsData(selectedItems, item => ({...item.data, icon: icon}));
        })
    }

    openRenameItemsDialog(items?: Item[]): void {
        const selectedItems = items || Object.values(this.selectedItemsByIds);
        const dialogRef = this.dialog.open<PromptDialogComponent, PromptDialogData, PromptDialogResult>(
            PromptDialogComponent, {
                data: {
                    confirmText: 'RENOMMER',
                    cancelText: 'ANNULER',
                    placeholder: 'Nom',
                    title: selectedItems.length > 1 ? 'Renommer les objets sélectionner' : 'Renommer l\'objet',
                    initialValue: selectedItems[0].data.name
                }
            });
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.updateItemsData(selectedItems, item => ({...item.data, name: result.text}));
        });
    }

    deleteSelectedItems(items?: Item[]) {
        const selectedItems = items || Object.values(this.selectedItemsByIds);
        this.deleteItems.emit(selectedItems);
        for (let item of selectedItems) {
            delete this.selectedItemsByIds[item.id];
        }
    }

    private updateItemsData(selectedItems, transformItemData: (item: Item) => IItemData) {
        for (let item of selectedItems) {
            const newItemData = transformItemData(item);
            if (this.itemsExistsOnServer) {
                this.itemService.updateItem(item.id, newItemData).subscribe((updatedItem) => {
                    item.data = new ItemData(updatedItem.data);
                });
            } else {
                item.data = new ItemData(newItemData);
            }
        }
    }
}
