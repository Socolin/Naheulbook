import {ChangeDetectionStrategy, Component, Input, OnChanges, OnInit, SimpleChanges} from '@angular/core';
import {isNullOrUndefined} from 'util';

import {Character} from './character.model';
import {Item, ItemService} from '../item';
import {ItemActionService} from './item-action.service';
import {MatDialog} from '@angular/material';
import {AddItemDialogComponent} from './add-item-dialog.component';
import {isItemNameMatchingFilter, ItemSortType, sortItemFn} from '../item/item-utils';
import {EditItemDialogComponent, EditItemDialogData, EditItemDialogResult} from './edit-item-dialog.component';
import {AddItemModifierDialogComponent} from './add-item-modifier-dialog.component';
import {ActiveStatsModifier} from '../shared';

@Component({
    selector: 'inventory-panel',
    templateUrl: './inventory-panel.component.html',
    styleUrls: ['./inventory-panel.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InventoryPanelComponent implements OnInit, OnChanges {
    @Input() character: Character;
    @Input() inGroupTab: boolean;

    public selectedItem?: Item;
    public sortType: ItemSortType = 'none';
    public viewMode: 'all' | 'bag' | 'money' | 'equipment' = 'bag';
    public itemFilterName?: string;

    get filteredItems(): Item[] {
        return this.character.items
            .filter(isItemNameMatchingFilter.bind(this, this.itemFilterName))
            .sort(sortItemFn.bind(this, this.sortType));
    }

    get topLevelBagItems(): Item[] {
        return this.character.computedData.topLevelContainers
            .concat(this.character.items.filter(i => !i.containerId && !i.data.equiped));
    }

    get moneyItems(): Item[] {
        return this.character.computedData.currencyItems;
    }

    constructor(
        private readonly itemService: ItemService,
        private readonly dialog: MatDialog,
        public readonly itemActionService: ItemActionService,
    ) {
    }

    ngOnChanges(changes: SimpleChanges): void {
    }

    openAddItemModal() {
        const dialogRef = this.dialog.open(AddItemDialogComponent, {minWidth: '100vw', height: '100vh'});

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.itemService.addItemTo('character', this.character.id
                , result.itemTemplateId
                , result.itemData)
                .subscribe(
                    item => {
                        this.character.onAddItem(item);
                        this.selectedItem = item;
                    }
                );
        })
    }

    deselectItem(): void {
        this.selectedItem = undefined;
    }

    selectItem(item: Item) {
        this.selectedItem = item;
        return false;
    }

    editItem(item: Item) {
        const dialogRef = this.dialog.open<EditItemDialogComponent, EditItemDialogData, EditItemDialogResult>(
            EditItemDialogComponent,
            {
                data: {item: item}
            }
        );
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }
            this.itemActionService.onAction('edit_item_name', item, {
                name: result.name,
                description: result.description
            });
        });
    }

    openModifierDialog(item: Item) {
        const dialogRef = this.dialog.open<AddItemModifierDialogComponent, any, ActiveStatsModifier>(AddItemModifierDialogComponent, {
            minWidth: '100vw',
            height: '100vh'
        });
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }

            if (item.modifiers === null) {
                item.modifiers = [];
            }
            this.activeModifier(result);
            item.modifiers.push(result);
            this.itemActionService.onAction('update_modifiers', item);
        });
    }

    activeModifier(modifier: ActiveStatsModifier) {
        modifier.active = true;
        switch (modifier.durationType) {
            case 'time':
                modifier.currentTimeDuration = modifier.timeDuration;
                break;
            case 'combat':
                modifier.currentCombatCount = modifier.combatCount;
                break;
            case 'lap':
                modifier.currentLapCount = modifier.lapCount;
                break;
        }
    }

    putItemInContainer(item: Item, container?: Item) {
        if (container) {
            if (item.id === container.id) {
                return;
            }
            this.itemActionService.onAction('move_to_container', item, {container: container.id});
        } else {
            this.itemActionService.onAction('move_to_container', item, {container: undefined});
        }
    }

    trackByItemId(item: Item): number {
        return item.id;
    }
    ngOnInit() {
        this.itemActionService.registerAction('equip').subscribe((event: { item: Item, data: any }) => {
            let eventData = event.data;
            let item = event.item;
            let level = 1;
            if (!isNullOrUndefined(eventData) && !isNullOrUndefined(eventData.level)) {
                level = eventData.level;
            }
            this.itemService.equipItem(item.id, level).subscribe(
                this.character.onEquipItem.bind(this.character)
            );
        });
        this.itemActionService.registerAction('unequip').subscribe((event: { item: Item, data: any }) => {
            let item = event.item;
            this.itemService.equipItem(item.id, 0).subscribe(
                this.character.onEquipItem.bind(this.character)
            );
        });
        this.itemActionService.registerAction('delete').subscribe((event: { item: Item, data: any }) => {
            let item = event.item;
            this.itemService.deleteItem(item.id).subscribe(
                () => {
                    if (this.selectedItem && this.selectedItem.id === item.id) {
                        this.selectedItem = undefined;
                    }
                    this.character.onDeleteItem(item.id);
                }
            );
        });
        this.itemActionService.registerAction('update_quantity').subscribe((event: { item: Item, data: any }) => {
            let eventData = event.data;
            let item = event.item as Item;
            let itemData = {
                ...item.data,
                quantity: eventData.quantity
            };
            if (item.template.data.charge && eventData.quantity === (item.data.quantity || 0) - 1) {
                itemData.charge = item.template.data.charge;
            }
            this.itemService.updateItem(item.id, itemData).subscribe(
                res => {
                    this.character.onUpdateItem(res);
                }
            );
        });
        this.itemActionService.registerAction('read_skill_book').subscribe((event: { item: Item, data: any }) => {
            let item = event.item;
            this.itemService.updateItem(item.id, {...item.data, readCount: (item.data.readCount || 0) + 1}).subscribe(
                res => {
                    this.character.onUpdateItem(res);
                }
            );
        });
        this.itemActionService.registerAction('identify').subscribe((event: { item: Item, data: any }) => {
            let item = event.item;
            let itemData = {...item.data, name: item.template.name};
            delete itemData.notIdentified;
            this.itemService.updateItem(item.id, itemData).subscribe((data) =>
                this.character.onUpdateItem(data)
            );
        });
        this.itemActionService.registerAction('ignoreRestrictions').subscribe((event: { item: Item, data: any }) => {
            let item = event.item;
            item.data = {
                ...item.data,
                ignoreRestrictions: event.data
            };
            this.itemService.updateItem(item.id, item.data).subscribe(
                this.character.onUpdateItem.bind(this.character)
            );
        });
        this.itemActionService.registerAction('use_charge').subscribe((event: { item: Item, data: any }) => {
            let item = event.item;
            if (item.data.charge == null) {
                item.data.charge = item.template.data.charge;
            }
            if (!item.data.charge) {
                throw new Error('Cannot use charge on item with no defined charge. itemId: ' + item.id);
            }
            this.itemService.useItemCharge(item.id).subscribe(
                this.character.onUpdateItem.bind(this.character)
            );
        });
        this.itemActionService.registerAction('move_to_container').subscribe((event: { item: Item, data: any }) => {
            let eventData = event.data;
            let item = event.item;
            this.itemService.moveToContainer(item.id, eventData.container).subscribe(
                this.character.onChangeContainer.bind(this.character)
            );
        });

        this.itemActionService.registerAction('edit_item_name').subscribe((event: { item: Item, data: any }) => {
            let eventData = event.data;
            let item = event.item;
            item.data = {
                ...item.data,
                name: eventData.name,
                description: eventData.description
            };
            this.itemService.updateItem(item.id, item.data).subscribe(
                this.character.onUpdateItem.bind(this.character)
            );
        });

        this.itemActionService.registerAction('give').subscribe((event: { item: Item, data: any }) => {
            let eventData = event.data;
            let item = event.item;
            this.itemService.giveItem(item.id, eventData.characterId, eventData.quantity).subscribe(
                result => {
                    if (result.remainingQuantity) {
                        if (this.selectedItem && this.selectedItem.id === item.id) {
                            this.selectedItem = undefined;
                        }
                        item.data.quantity = result.remainingQuantity;
                    } else {
                        this.character.onDeleteItem(item.id);
                    }
                }
            );
        });

        this.itemActionService.registerAction('change_icon').subscribe((event: { item: Item, data: any }) => {
            let eventData = event.data;
            let item = event.item;
            item.data = {
                ...item.data,
                icon: eventData.icon
            };
            this.itemService.updateItem(item.id, item.data).subscribe(
                this.character.onUpdateItem.bind(this.character)
            );
        });

        this.itemActionService.registerAction('update_modifiers').subscribe((event: { item: Item, data: any }) => {
            let item = event.item;
            this.itemService.updateItemModifiers(item.id, item.modifiers).subscribe(
                this.character.onUpdateModifiers.bind(this.character)
            );
        });

        this.itemActionService.registerAction('update_data').subscribe((event: { item: Item, data: any }) => {
            let item = event.item;
            this.itemService.updateItem(item.id, item.data).subscribe(
                this.character.onUpdateItem.bind(this.character)
            );
        });
    }
}
