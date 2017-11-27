import {
    Input, OnInit, Component, HostListener, ElementRef, ViewChild,
    OnChanges, SimpleChanges
} from '@angular/core';
import {Overlay, OverlayRef, OverlayConfig} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';
import {isNullOrUndefined} from 'util';

import {removeDiacritics} from '../shared';
import {AutocompleteSearchItemTemplateComponent, ItemTemplate} from '../item-template';

import {Character} from './character.model';
import {Item, ItemData, ItemService} from '../item';
import {ItemActionService} from './item-action.service';
import {SwipeService} from './swipe.service';

@Component({
    selector: 'inventory-panel',
    templateUrl: './inventory-panel.component.html',
    styleUrls: ['./inventory-panel.component.scss'],
    providers: [SwipeService],
})
export class InventoryPanelComponent implements OnInit, OnChanges {
    @Input() character: Character;
    @Input() inGroupTab: boolean;

    // Inventory
    public selectedItem: Item | undefined;
    public selectedInventoryTab = 'all';
    public sortType = 'none';

    public iconMode = false;
    public itemFilterName: string;

    @ViewChild('itemDetail')
    private itemDetailDiv: ElementRef;
    @ViewChild('itemDetailTop')
    private itemDetailTopDiv: ElementRef;
    public itemDetailOffset = 0;

    @ViewChild('addItemDialog')
    public addItemDialog: Portal<any>;
    public addItemOverlayRef: OverlayRef | undefined;
    public addItemSearch: string | undefined;
    public filteredItems: ItemTemplate[] = [];

    @ViewChild('autocompleteSearchItemTemplate')
    public autocompleteSearchItemTemplate: AutocompleteSearchItemTemplateComponent;

    public itemAddCustomName: string | undefined;
    public itemAddCustomDescription: string | undefined;
    public selectedAddItem: ItemTemplate | undefined;
    public itemAddQuantity: number | undefined;


    constructor(
        private _itemService: ItemService
        , private _overlay: Overlay
        , public _itemActionService: ItemActionService) {
    }

    @HostListener('window:scroll') onScroll(): boolean {
        if (this.itemDetailDiv && this.itemDetailTopDiv) {
            let rect = this.itemDetailTopDiv.nativeElement.getBoundingClientRect();
            if (rect.top < 30) {
                this.itemDetailOffset = 30 - rect.top;
            } else {
                this.itemDetailOffset = 0;
            }
        }
        return true;
    }

    ngOnChanges(changes: SimpleChanges): void {
    }

    toggleIconMode() {
        this.iconMode = !this.iconMode;
    }

    selectAddItem(item: ItemTemplate) {
        this.selectedAddItem = item;
        this.itemAddCustomName = item.name;
        if (item.data.quantifiable) {
            this.itemAddQuantity = 1;
        } else {
            this.itemAddQuantity = undefined;
        }
        return false;
    }

    openAddItemModal() {
        this.addItemSearch = undefined;
        this.filteredItems = [];

        let config = new OverlayConfig();

        config.positionStrategy = this._overlay.position()
            .global()
            .centerHorizontally()
            .top('32px');
        config.hasBackdrop = true;

        let overlayRef = this._overlay.create(config);
        overlayRef.attach(this.addItemDialog);
        overlayRef.backdropClick().subscribe(() => overlayRef.detach());
        this.addItemOverlayRef = overlayRef;
        setTimeout(() => {
            this.autocompleteSearchItemTemplate.focus();
        }, 0);
    }

    closeAddItemDialog() {
        if (!this.addItemOverlayRef) {
            return;
        }
        this.addItemOverlayRef.detach();
        this.addItemOverlayRef = undefined;
    }

    trackById(index, element) {
        return element.id;
    }

    addItem() {
        if (!this.selectedAddItem) {
            return;
        }
        if (!this.itemAddCustomName) {
            return;
        }
        let itemData = new ItemData();
        itemData.name = this.itemAddCustomName;
        itemData.description = this.itemAddCustomDescription;
        itemData.quantity = this.itemAddQuantity;
        this._itemService.addItemTo('character', this.character.id
            , this.selectedAddItem.id
            , itemData)
            .subscribe(
                item => {
                    this.character.onAddItem(item);
                    this.selectedItem = item;
                    this.addItemSearch = '';
                    this.selectedAddItem = undefined;
                    this.itemAddCustomName = undefined;
                    this.itemAddCustomDescription = undefined;
                    this.itemAddQuantity = undefined;
                }
            );
        this.closeAddItemDialog();
    }

    selectItem(item: Item) {
        this.selectedItem = item;
        return false;
    }

    isItemFilteredByName(item: Item): boolean {
        if (!this.itemFilterName) {
            return true;
        }

        let cleanFilter = removeDiacritics(this.itemFilterName).toLowerCase();
        if (removeDiacritics(item.data.name).toLowerCase().indexOf(cleanFilter) > -1) {
            return true;
        }
        if (removeDiacritics(item.template.name).toLowerCase().indexOf(cleanFilter) > -1) {
            return true;
        }
        return false;
    }

    sortInventory(type?: string) {
        if (type === 'not_identified_first') {
            this.sortType = type;
            this.character.items.sort((a, b) => {
                    if (a.data.notIdentified && b.data.notIdentified) {
                        return 0;
                    }
                    if (a.data.notIdentified) {
                        return -1;
                    }
                    if (b.data.notIdentified) {
                        return 1;
                    }

                    if ((a.container || a.data.equiped) && (b.container || b.data.equiped)) {
                        return a.data.name.localeCompare(b.data.name);
                    }
                    if (a.container || a.data.equiped) {
                        return 1;
                    }
                    if (b.container || b.data.equiped) {
                        return -1;
                    }

                    return a.data.name.localeCompare(b.data.name);
                }
            );

        }
        else {
            if (this.sortType !== 'asc') {
                this.sortType = 'asc';
                this.character.items.sort((a, b) => {
                        return a.data.name.localeCompare(b.data.name);
                    }
                );
            }
            else {
                this.sortType = 'desc';
                this.character.items.sort((a, b) => {
                        return 2 - a.data.name.localeCompare(b.data.name);
                    }
                );
            }
            this.character.update();
        }
    }

    ngOnInit() {
        this._itemActionService.registerAction('equip').subscribe((event: {item: Item, data: any}) => {
            let eventData = event.data;
            let item = event.item;
            let level = 1;
            if (!isNullOrUndefined(eventData) && !isNullOrUndefined(eventData.level)) {
                level = eventData.level;
            }
            this._itemService.equipItem(item.id, level).subscribe(
                this.character.onEquipItem.bind(this.character)
            );
        });
        this._itemActionService.registerAction('unequip').subscribe((event: {item: Item, data: any}) => {
            let item = event.item;
            this._itemService.equipItem(item.id, 0).subscribe(
                this.character.onEquipItem.bind(this.character)
            );
        });
        this._itemActionService.registerAction('delete').subscribe((event: {item: Item, data: any}) => {
            let item = event.item;
            this._itemService.deleteItem(item.id).subscribe(
                deletedItem => {
                    if (this.selectedItem && this.selectedItem.id === item.id) {
                        this.selectedItem = undefined;
                    }
                    this.character.onDeleteItem(deletedItem);
                }
            );
        });
        this._itemActionService.registerAction('update_quantity').subscribe((event: {item: Item, data: any}) => {
            let eventData = event.data;
            let item = event.item;
            this._itemService.updateQuantity(item.id, eventData.quantity).subscribe(
                res => {
                    this.character.onUpdateQuantity(res);
                }
            );
        });
        this._itemActionService.registerAction('read_skill_book').subscribe((event: {item: Item, data: any}) => {
            let item = event.item;
            this._itemService.readBook(item.id).subscribe(
                res => {
                    item.data.readCount = res.data.readCount;
                    this.character.update();
                }
            );
        });
        this._itemActionService.registerAction('identify').subscribe((event: {item: Item, data: any}) => {
            let item = event.item;
            this._itemService.identify(item.id).subscribe(
                this.character.onIdentifyItem.bind(this.character)
            );
        });
        this._itemActionService.registerAction('ignoreRestrictions').subscribe((event: {item: Item, data: any}) => {
            let item = event.item;
            item.data.ignoreRestrictions = event.data;
            this._itemService.updateItem(item.id, item.data).subscribe(
                this.character.onUpdateItem.bind(this.character)
            );
        });
        this._itemActionService.registerAction('use_charge').subscribe((event: {item: Item, data: any}) => {
            let item = event.item;
            if (item.data.charge != null) {
                item.data.charge = item.template.data.charge;
            }
            if (!item.data.charge) {
                throw new Error('Cannot use charge on item whith no defined charge. itemId: ' + item.id);
            }
            this._itemService.updateCharge(item.id, item.data.charge - 1).subscribe(
                this.character.onUseItemCharge.bind(this.character)
            );
        });
        this._itemActionService.registerAction('move_to_container').subscribe((event: {item: Item, data: any}) => {
            let eventData = event.data;
            let item = event.item;
            this._itemService.moveToContainer(item.id, eventData.container).subscribe(
                this.character.onChangeContainer.bind(this.character)
            );
        });

        this._itemActionService.registerAction('edit_item_name').subscribe((event: {item: Item, data: any}) => {
            let eventData = event.data;
            let item = event.item;
            let data = {
                name: eventData.name,
                description: eventData.description
            };
            this._itemService.updateItem(item.id, data).subscribe(
                this.character.onUpdateItem.bind(this.character)
            );
        });

        this._itemActionService.registerAction('give').subscribe((event: {item: Item, data: any}) => {
            let eventData = event.data;
            let item = event.item;
            this._itemService.giveItem(item.id, eventData.characterId, eventData.quantity).subscribe(
                givenItem => {
                    // FIXME Do not unselect item if only a part of the stack is given
                    if (this.selectedItem && this.selectedItem.id === item.id) {
                        this.selectedItem = undefined;
                    }
                    this.character.onDeleteItem(givenItem);
                }
            );
        });

        this._itemActionService.registerAction('change_icon').subscribe((event: {item: Item, data: any}) => {
            let eventData = event.data;
            let item = event.item;
            item.data.icon = eventData.icon;
            this._itemService.updateItem(item.id, item.data).subscribe(
                this.character.onUpdateItem.bind(this.character)
            );
        });

        this._itemActionService.registerAction('update_modifiers').subscribe((event: {item: Item, data: any}) => {
            let item = event.item;
            this._itemService.updateItemModifiers(item.id, item.modifiers).subscribe(
                this.character.onUpdateModifiers.bind(this.character)
            );
        });

        this._itemActionService.registerAction('update_data').subscribe((event: {item: Item, data: any}) => {
            let item = event.item;
            this._itemService.updateItem(item.id, item.data).subscribe(
                this.character.onUpdateItem.bind(this.character)
            );
        });
    }
}
