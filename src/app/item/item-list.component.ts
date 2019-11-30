import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {Item, ItemData} from './item.model';
import {MatCheckbox} from '@angular/material/checkbox';
import {IconDescription} from '../shared/icon.model';
import {
    IconSelectorComponent,
    IconSelectorComponentDialogData,
    PromptDialogComponent,
    PromptDialogData, PromptDialogResult
} from '../shared';
import {ItemService} from './item.service';
import {NhbkMatDialog} from '../material-workaround';

export type MultiSelectItemActionsNames = 'changeIcon' | 'rename' | 'delete';

@Component({
    selector: 'app-item-list',
    templateUrl: './item-list.component.html',
    styleUrls: ['./item-list.component.scss']
})
export class ItemListComponent {
    @Input()
    public items: Item[];
    @Input()
    public actionIcon = 'delete';
    @Input()
    public actionName = 'Supprimer';
    @Output()
    public onAction: EventEmitter<Item> = new EventEmitter<Item>();
    @Output()
    public selectItem: EventEmitter<Item> = new EventEmitter<Item>();
    @Input()
    public multiSelectActions: (MultiSelectItemActionsNames)[] = [];
    @Output()
    public multiSelectionChange: EventEmitter<Item[]> = new EventEmitter<Item[]>();
    @Output()
    public multiSelectAction: EventEmitter<{ action: MultiSelectItemActionsNames, items: Item[] }> =
        new EventEmitter<{ action: MultiSelectItemActionsNames, items: Item[] }>();

    @ViewChild('selectAllCheckbox', {static: false})
    private selectAllCheckbox: MatCheckbox;
    public selectedItemsByIds: { [itemId: number]: Item } = {};
    public multiSelectActionDefinitions: {
        [name: string]: {
            label?: string,
            iconSet?: string,
            icon?: string,
            materialIcon?: string,
            tooltip?: string
        }
    } = {
        'changeIcon': {
            iconSet: 'game-icon',
            icon: 'game-icon-card-exchange',
            tooltip: 'Changer l\'icône'
        },
        'rename': {
            materialIcon: 'edit',
            tooltip: 'Renommer'
        },
        'delete': {
            label: 'Supprimer',
            materialIcon: 'delete'
        }
    };

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
        this.multiSelectionChange.emit(Object.values(this.selectedItemsByIds));
    }

    toggleSelectItem(item: Item, checked: boolean) {
        if (checked) {
            this.selectedItemsByIds[item.id] = item;
        } else {
            delete this.selectedItemsByIds[item.id];
        }
        this.multiSelectionChange.emit(Object.values(this.selectedItemsByIds));

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

    executeMultiSelectAction(action: MultiSelectItemActionsNames) {
        const selectedItems = Object.values(this.selectedItemsByIds);
        switch (action) {
            case 'changeIcon':
                this.openSelectIconDialog(selectedItems[0].data.icon, selectedItems);
                break;
            case 'rename':
                this.openRenameItemsDialog(selectedItems[0].data.name, selectedItems);
                break;
            default:
                this.multiSelectAction.emit({action: action, items: selectedItems});
                break;
        }
    }

    openSelectIconDialog(currentIcon: IconDescription | undefined, items: Item[]): void {
        const dialogRef = this.dialog.open<IconSelectorComponent, IconSelectorComponentDialogData, IconDescription>(IconSelectorComponent, {
            data: {icon: currentIcon}
        });

        dialogRef.afterClosed().subscribe((icon) => {
            if (!icon) {
                return;
            }
            for (let item of items) {
                this.itemService.updateItem(item.id, {...item.data, icon: icon}).subscribe((updatedItem) => {
                    item.data = new ItemData(updatedItem.data);
                });
            }
        })
    }

    private openRenameItemsDialog(currentItemName: string, items: Item[]) {
        const dialogRef = this.dialog.open<PromptDialogComponent, PromptDialogData, PromptDialogResult>(
            PromptDialogComponent, {
                data: {
                    confirmText: 'RENOMMER',
                    cancelText: 'ANNULER',
                    placeholder: 'Nom',
                    title: 'Renommer les objets sélectionner',
                    initialValue: currentItemName
                }
            });
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            for (let item of items) {
                this.itemService.updateItem(item.id, {...item.data, name: result.text}).subscribe((updatedItem) => {
                    item.data = new ItemData(updatedItem.data);
                });
            }
        });
    }
}
