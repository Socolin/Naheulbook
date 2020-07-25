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
export class ItemListComponent implements OnChanges {
    @Input()
    public items: Item[];
    @Input()
    public itemsExistsOnServer = true;
    @Output()
    public selectItem: EventEmitter<Item> = new EventEmitter<Item>();
    @Output()
    public deleteItems: EventEmitter<Item[]> = new EventEmitter<Item[]>();

    @ViewChild('selectAllCheckbox')
    private selectAllCheckbox?: MatCheckbox;
    public selectedItems: Item[] = [];

    constructor(
        private readonly itemService: ItemService,
        private readonly dialog: NhbkMatDialog,
    ) {
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ('items' in changes) {
            this.toggleSelectAll(false);
        }
    }

    toggleSelectAll(checked: boolean) {
        if (checked) {
            this.selectedItems = [...this.items];
        } else {
            this.selectedItems = [];
        }
        this.updateSelectAllCheckboxState();
    }

    toggleSelectItem(item: Item, checked: boolean) {
        if (checked) {
            if (this.selectedItems.indexOf(item) === -1) {
                this.selectedItems.push(item);
            }
        } else {
            const index = this.selectedItems.indexOf(item);
            if (index !== -1) {
                this.selectedItems.splice(index, 1);
            }
        }

        this.updateSelectAllCheckboxState();
    }

    markItemsAsNotIdentified(items?: Item[]) {
        const selectedItems = items || this.selectedItems;
        this.updateItemsData(selectedItems, (item => ({
            ...item.data,
            notIdentified: true,
            name: item.template.data.notIdentifiedName || item.template.name
        })));
    }

    identifyItems(items?: Item[]) {
        const selectedItems = items || this.selectedItems;
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
        const selectedItems = items || this.selectedItems;
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
        const selectedItems = items || this.selectedItems;
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
        const selectedItems = items || [...this.selectedItems];
        this.deleteItems.emit(selectedItems);
        for (const item of selectedItems) {
            this.toggleSelectItem(item, false);
        }
        this.updateSelectAllCheckboxState();
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

    private updateSelectAllCheckboxState() {
        if (!this.selectAllCheckbox) {
            return;
        }
        if (this.selectedItems.length === 0) {
            this.selectAllCheckbox.checked = false;
            this.selectAllCheckbox.indeterminate = false;
        } else if (this.selectedItems.length === this.items.length) {
            this.selectAllCheckbox.checked = true;
            this.selectAllCheckbox.indeterminate = false;
        } else {
            this.selectAllCheckbox.checked = true;
            this.selectAllCheckbox.indeterminate = true;
        }
    }
}
