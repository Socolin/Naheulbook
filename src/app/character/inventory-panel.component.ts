import {
    ChangeDetectionStrategy,
    ChangeDetectorRef,
    Component,
    Input,
    OnChanges,
    OnDestroy,
    OnInit,
    SimpleChanges
} from '@angular/core';

import {Character} from './character.model';
import {Item, ItemService} from '../item';
import {ItemActionService} from './item-action.service';
import {AddItemDialogComponent} from './add-item-dialog.component';
import {isItemNameMatchingFilter, ItemSortType, sortItemFn} from '../item/item-utils';
import {EditItemDialogComponent, EditItemDialogData, EditItemDialogResult} from './edit-item-dialog.component';
import {AddItemModifierDialogComponent} from './add-item-modifier-dialog.component';
import {ActiveStatsModifier, IconSelectorComponent, IconSelectorComponentDialogData} from '../shared';
import {ItemLifetimeEditorDialogComponent, ItemLifetimeEditorDialogData} from './item-lifetime-editor-dialog.component';
import {IDurable} from '../api/shared';
import {GiveItemDialogComponent, GiveItemDialogData, GiveItemDialogResult} from './give-item-dialog.component';
import {Subscription} from 'rxjs';
import {IconDescription} from '../shared/icon.model';
import {NhbkMatDialog} from '../material-workaround';

@Component({
    selector: 'inventory-panel',
    templateUrl: './inventory-panel.component.html',
    styleUrls: ['./inventory-panel.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InventoryPanelComponent implements OnInit, OnChanges, OnDestroy {
    @Input() character: Character;
    @Input() inGroupTab: boolean;

    public sortType: ItemSortType = 'none';
    public viewMode: 'all' | 'bag' | 'money' | 'equipment' = 'bag';
    public itemFilterName?: string;

    private subscription: Subscription = new Subscription();

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
        private readonly changeDetectorRef: ChangeDetectorRef,
        private readonly itemService: ItemService,
        private readonly dialog: NhbkMatDialog,
        public readonly itemActionService: ItemActionService,
    ) {
    }

    ngOnChanges(changes: SimpleChanges): void {
    }

    openAddItemModal() {
        const dialogRef = this.dialog.openFullScreen(AddItemDialogComponent);

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
                    }
                );
        })
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
        const dialogRef = this.dialog.openFullScreen<AddItemModifierDialogComponent, any, ActiveStatsModifier>(
            AddItemModifierDialogComponent
        );
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }

            this.activeModifier(result);
            this.itemActionService.onAction('update_modifiers', item, [...item.modifiers, result]);
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
            this.itemActionService.onAction('move_to_container', item, {containerId: container.id});
        } else {
            this.itemActionService.onAction('move_to_container', item, {containerId: undefined});
        }
    }

    trackByItemId(item: Item): number {
        return item.id;
    }

    ngOnInit() {
        this.subscription.add(this.itemActionService.registerAction('equip').subscribe(event => {
            let level = 1;
            if (event.data && event.data.level) {
                level = event.data.level;
            }
            this.itemService.equipItem(event.item.id, level).subscribe(
                this.character.onEquipItem.bind(this.character)
            );
        }));
        this.subscription.add(this.itemActionService.registerAction('unequip').subscribe(event => {
            this.itemService.equipItem(event.item.id, 0).subscribe(
                this.character.onEquipItem.bind(this.character)
            );
        }));
        this.subscription.add(this.itemActionService.registerAction('delete').subscribe(event => {
            let item = event.item;
            this.itemService.deleteItem(item.id).subscribe(
                () => {
                    this.character.onDeleteItem(item.id);
                }
            );
        }));
        this.subscription.add(this.itemActionService.registerAction('update_quantity').subscribe(event => {
            let itemData = {
                ...event.item.data,
                quantity: event.data.quantity
            };
            if (event.item.template.data.charge && event.data.quantity === (event.item.data.quantity || 0) - 1) {
                itemData.charge = event.item.template.data.charge;
            }
            this.itemService.updateItem(event.item.id, itemData).subscribe(
                res => {
                    this.character.onUpdateItem(res);
                }
            );
        }));
        this.subscription.add(this.itemActionService.registerAction('read_skill_book').subscribe(event => {
            this.itemService.updateItem(event.item.id, {...event.item.data, readCount: (event.item.data.readCount || 0) + 1}).subscribe(
                res => {
                    this.character.onUpdateItem(res);
                }
            );
        }));
        this.subscription.add(this.itemActionService.registerAction('identify').subscribe(event => {
            let itemData = {...event.item.data, name: event.item.template.name};
            delete itemData.notIdentified;
            this.itemService.updateItem(event.item.id, itemData).subscribe((data) =>
                this.character.onUpdateItem(data)
            );
        }));
        this.subscription.add(this.itemActionService.registerAction('ignore_restrictions').subscribe(event => {
            let item = event.item;
            item.data = {
                ...item.data,
                ignoreRestrictions: event.data
            };
            this.itemService.updateItem(item.id, item.data).subscribe(
                this.character.onUpdateItem.bind(this.character)
            );
        }));
            this.subscription.add(this.itemActionService.registerAction('use_charge').subscribe(event => {
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
        }));
        this.subscription.add(this.itemActionService.registerAction('move_to_container').subscribe(event => {
            let eventData = event.data;
            let item = event.item;
            this.itemService.moveToContainer(item.id, eventData.containerId).subscribe(
                this.character.onChangeContainer.bind(this.character)
            );
        }));

        this.subscription.add(this.itemActionService.registerAction('edit_item_name').subscribe(event => {
            event.item.data = {
                ...event.item.data,
                name: event.data.name,
                description: event.data.description
            };
            this.itemService.updateItem(event.item.id, event.item.data).subscribe(
                this.character.onUpdateItem.bind(this.character)
            );
        }));

        this.subscription.add(this.itemActionService.registerAction('give').subscribe(event => {
            this.itemService.giveItem(event.item.id, event.data.characterId, event.data.quantity).subscribe(
                result => {
                    if (result.remainingQuantity) {
                        event.item.data.quantity = result.remainingQuantity;
                    } else {
                        this.character.onDeleteItem(event.item.id);
                    }
                }
            );
        }));

        this.subscription.add(this.itemActionService.registerAction('change_icon').subscribe(event => {
            let eventData = event.data;
            let item = event.item;
            item.data = {
                ...item.data,
                icon: eventData.icon
            };
            this.itemService.updateItem(item.id, item.data).subscribe(
                this.character.onUpdateItem.bind(this.character)
            );
        }));

        this.subscription.add(this.itemActionService.registerAction('update_modifiers').subscribe(event => {
            this.itemService.updateItemModifiers(event.item.id, event.data).subscribe(
                this.character.onUpdateModifiers.bind(this.character)
            );
        }));

        this.subscription.add(this.itemActionService.registerAction('update_data').subscribe(event => {
            this.itemService.updateItem(event.item.id, event.data).subscribe(partialItem => this.character.onUpdateItem(partialItem));
        }));

        this.subscription.add(this.character.onUpdate.subscribe(() => {
            this.changeDetectorRef.detectChanges();
        }))
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe();
    }

    openLifetimeDialog(item: Item): void {
        const dialogRef = this.dialog.open<ItemLifetimeEditorDialogComponent, ItemLifetimeEditorDialogData, IDurable>(
            ItemLifetimeEditorDialogComponent,
            {
                data: {lifetime: item.data.lifetime}
            });
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            const newLifeTime = result.durationType !== 'forever' ? result : undefined;
            this.itemActionService.onAction('update_data', item, {...item.data, lifetime: newLifeTime});
        })
    }

    openGiveItemDialog(item: Item): void {
        const dialogRef = this.dialog.open<GiveItemDialogComponent, GiveItemDialogData, GiveItemDialogResult>(
            GiveItemDialogComponent, {
                autoFocus: false,
                data: {
                    item: item,
                    character: this.character
                }
            });
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.itemActionService.onAction('give', item, {
                characterId: result.giveTarget.id,
                quantity: result.giveQuantity
            });
        });
    }

    openSelectItemIconDialog(item: Item): void {
        const dialogRef = this.dialog.open<IconSelectorComponent, IconSelectorComponentDialogData, IconDescription>(IconSelectorComponent, {
            autoFocus: false,
            data: {icon: item.data.icon}
        });

        dialogRef.afterClosed().subscribe((icon) => {
            if (!icon) {
                return;
            }
            this.itemActionService.onAction('change_icon', item, {icon: icon});
        })
    }
}
