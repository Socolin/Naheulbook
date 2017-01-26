import {
    Input, OnInit, Component, Inject, forwardRef, HostListener, ElementRef, ViewChild,
    OnChanges, SimpleChanges
} from '@angular/core';
import {OverlayRef, Portal, Overlay, OverlayState} from '@angular/material';

import {Character} from './character.model';
import {CharacterWebsocketService} from './character-websocket.service';
import {ItemService} from '../item/item.service';
import {ItemTemplate} from '../item/item-template.model';
import {Item, PartialItem, ItemData} from './item.model';
import {removeDiacritics} from '../shared/remove_diacritics';
import {SwipeService} from './swipe.service';
import {ItemActionService} from './item-action.service';
import {isNullOrUndefined} from 'util';
import {Observable} from 'rxjs';
import {AutocompleteValue} from '../shared/autocomplete-input.component';

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
    public selectedItem: Item;
    public selectedInventoryTab: string = 'all';
    private sortType: string = 'none';

    public iconMode: boolean = false;
    public itemFilterName: string;

    @ViewChild('itemDetail')
    private itemDetailDiv: ElementRef;
    @ViewChild('itemDetailTop')
    private itemDetailTopDiv: ElementRef;
    public itemDetailOffset: number = 0;

    @ViewChild('addItemDialog')
    public addItemDialog: Portal<any>;
    public addItemOverlayRef: OverlayRef;
    public addItemSearch: string;
    public filteredItems: ItemTemplate[] = [];

    public autocompleteAddItemListCallback: Function = this.updateItemListAutocomplete.bind(this);

    public itemAddCustomName: string;
    public itemAddCustomDescription: string;
    public selectedAddItem: ItemTemplate;
    public itemAddQuantity: number;

    constructor(
        @Inject(forwardRef(()  => ItemService)) private _itemService: ItemService
        , private _characterWebsocketService: CharacterWebsocketService
        , private _overlay: Overlay
        , private _itemActionService: ItemActionService) {
    }

    @HostListener('window:scroll') onScroll(): boolean {
        if (this.itemDetailDiv && this.itemDetailTopDiv) {
            let rect = this.itemDetailTopDiv.nativeElement.getBoundingClientRect();
            if (rect.top < 30) {
                this.itemDetailOffset = 30 - rect.top;
            } else {
                this.itemDetailOffset = 0;
            }
            return true;
        }
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
            this.itemAddQuantity = null;
        }
        return false;
    }

    onAddItem(item: Item) {
        for (let i = 0; i < this.character.items.length; i++) {
            if (this.character.items[i].id === item.id) {
                return;
            }
        }

        this._characterWebsocketService.notifyChange('Ajout de l\'objet: ' + item.data.name);
        this.character.items.push(item);
        this.character.update();
    }

    openAddItemModal() {
        this.addItemSearch = null;
        this.filteredItems = [];

        let config = new OverlayState();

        config.positionStrategy = this._overlay.position()
            .global()
            .centerHorizontally()
            .centerVertically();
        config.hasBackdrop = true;

        let overlayRef = this._overlay.create(config);
        overlayRef.attach(this.addItemDialog);
        overlayRef.backdropClick().subscribe(() => overlayRef.detach());
        this.addItemOverlayRef = overlayRef;
    }

    closeAddItemDialog() {
        this.addItemOverlayRef.detach();
    }

    updateItemListAutocomplete(filter: string): Observable<AutocompleteValue[]> {
        this.addItemSearch = filter;
        if (filter === '') {
            return Observable.from([]);
        }
        return this._itemService.searchItem(this.addItemSearch).map(
            items => {
                return items.map(e => new AutocompleteValue(e, e.name));
            }
        );
    }

    addItem() {
        let itemData = new ItemData();
        itemData['name'] = this.itemAddCustomName;
        itemData['description'] = this.itemAddCustomDescription;
        itemData['quantity'] = this.itemAddQuantity;
        this._itemService.addItemTo('character', this.character.id
            , this.selectedAddItem.id
            , itemData)
            .subscribe(
                item => {
                    this.onAddItem(item);
                    this.selectedItem = item;
                    this.addItemSearch = '';
                    this.selectedAddItem = null;
                    this.itemAddCustomName = null;
                    this.itemAddCustomDescription = null;
                    this.itemAddQuantity = null;
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

    sortInventory(type: string) {
        if (type === 'not_identified_first') {
            this.sortType = type;
            this.character.items.sort((a, b) =>
                {
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

    onEquipItem(it: PartialItem) {
        for (let i = 0; i < this.character.items.length; i++) {
            let item = this.character.items[i];
            if (item.id === it.id) {
                if (item.data.equiped === it.data.equiped) {
                    return;
                }
                item.data.equiped = it.data.equiped;
                if (it.data.equiped) {
                    this._characterWebsocketService.notifyChange('Equipe ' + item.data.name);
                } else {
                    this._characterWebsocketService.notifyChange('Déséquipe ' + item.data.name);
                }
                this.character.update();
                return;
            }
        }
    }

    onDeleteItem(item: PartialItem) {
        if (this.selectedItem && this.selectedItem.id === item.id) {
            this.selectedItem = null;
        }
        for (let i = 0; i < this.character.items.length; i++) {
            let it = this.character.items[i];
            if (it.id === item.id) {
                this.character.items.splice(i, 1);
                this.character.update();
                this._characterWebsocketService.notifyChange('Suppression de l\'objet: ' + item.data.name);
                break;
            }
        }
    }

    onUseItemCharge(item: PartialItem) {
        for (let i = 0; i < this.character.items.length; i++) {
            let it = this.character.items[i];
            if (it.id === item.id) {
                it.data.charge = item.data.charge;
                this.character.update();
                this._characterWebsocketService.notifyChange('Utilisation d\'une charge de l\'objet: ' + item.data.name);
                break;
            }
        }
    }

    onChangeContainer(item: PartialItem) {
        for (let i = 0; i < this.character.items.length; i++) {
            let it = this.character.items[i];
            if (it.id === item.id) {
                it.container = item.container;
                this.character.update();
                break;
            }
        }
    }

    onIdentifyItem(item: PartialItem) {
        for (let i = 0; i < this.character.items.length; i++) {
            let it = this.character.items[i];
            if (it.id === item.id) {
                it.data.name = item.data.name;
                it.data.notIdentified = item.data.notIdentified;
                this.character.update();
                this._characterWebsocketService.notifyChange('Identification de l\'objet: ' + item.data.name);
                break;
            }
        }
    }

    onUpdateItem(item: PartialItem) {
        for (let i = 0; i < this.character.items.length; i++) {
            let it = this.character.items[i];
            if (it.id === item.id) {
                it.data = item.data;
                this.character.update();
                break;
            }
        }
    }

    onUpdateModifiers(item: PartialItem) {
        for (let i = 0; i < this.character.items.length; i++) {
            let it = this.character.items[i];
            if (it.id === item.id) {
                it.modifiers = item.modifiers;
                this.character.update();
                break;
            }
        }
    }

    onUpdateQuantity(item: PartialItem) {
        for (let i = 0; i < this.character.items.length; i++) {
            let it = this.character.items[i];
            if (it.id === item.id) {
                if (it.data.quantity !== item.data.quantity) {
                    this._characterWebsocketService.notifyChange('Modification de la quantité de l\'objet: '
                        + item.data.name + ': ' + it.data.quantity + ' ->' + item.data.quantity);
                    it.data.quantity = item.data.quantity;
                    it.data.charge = item.data.charge;
                    this.character.update();
                }
                break;
            }
        }
    }

    ngOnInit() {
        this._characterWebsocketService.registerPacket('equipItem').subscribe(this.onEquipItem.bind(this));
        this._characterWebsocketService.registerPacket('addItem').subscribe(this.onAddItem.bind(this));
        this._characterWebsocketService.registerPacket('deleteItem').subscribe(this.onDeleteItem.bind(this));
        this._characterWebsocketService.registerPacket('identifyItem').subscribe(this.onIdentifyItem.bind(this));
        this._characterWebsocketService.registerPacket('useCharge').subscribe(this.onUseItemCharge.bind(this));
        this._characterWebsocketService.registerPacket('changeContainer').subscribe(this.onChangeContainer.bind(this));
        this._characterWebsocketService.registerPacket('updateItem').subscribe(this.onUpdateItem.bind(this));
        this._characterWebsocketService.registerPacket('changeQuantity').subscribe(this.onUpdateQuantity.bind(this));
        this._characterWebsocketService.registerPacket('updateItemModifiers').subscribe(this.onUpdateModifiers.bind(this));

        this._itemActionService.registerAction('equip').subscribe((event: {item: Item, data: any}) => {
            let eventData = event.data;
            let item = event.item;
            let level = 1;
            if (!isNullOrUndefined(eventData) && !isNullOrUndefined(eventData.level)) {
                level = eventData.level;
            }
            this._itemService.equipItem(item.id, level).subscribe(
                this.onEquipItem.bind(this)
            );
        });
        this._itemActionService.registerAction('unequip').subscribe((event: {item: Item, data: any}) => {
            let item = event.item;
            this._itemService.equipItem(item.id, 0).subscribe(
                this.onEquipItem.bind(this)
            );
        });
        this._itemActionService.registerAction('delete').subscribe((event: {item: Item, data: any}) => {
            let item = event.item;
            this._itemService.deleteItem(item.id).subscribe(
                deletedItem => {
                    this.onDeleteItem(deletedItem);
                    this.selectedItem = null;
                }
            );
        });
        this._itemActionService.registerAction('update_quantity').subscribe((event: {item: Item, data: any}) => {
            let eventData = event.data;
            let item = event.item;
            this._itemService.updateQuantity(item.id, eventData.quantity).subscribe(
                res => {
                    this.onUpdateQuantity(res);
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
                this.onIdentifyItem.bind(this)
            );
        });
        this._itemActionService.registerAction('use_charge').subscribe((event: {item: Item, data: any}) => {
            let item = event.item;
            if (isNullOrUndefined(item.data.charge)) {
                item.data.charge = item.template.data.charge;
            }
            this._itemService.updateCharge(item.id, item.data.charge - 1).subscribe(
                this.onUseItemCharge.bind(this)
            );
        });
        this._itemActionService.registerAction('move_to_container').subscribe((event: {item: Item, data: any}) => {
            let eventData = event.data;
            let item = event.item;
            this._itemService.moveToContainer(item.id, eventData.container).subscribe(
                this.onChangeContainer.bind(this)
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
                this.onUpdateItem.bind(this)
            );
        });

        this._itemActionService.registerAction('give').subscribe((event: {item: Item, data: any}) => {
            let eventData = event.data;
            let item = event.item;
            this._itemService.giveItem(item.id, eventData.characterId).subscribe(
                this.onDeleteItem.bind(this)
            );
        });

        this._itemActionService.registerAction('change_icon').subscribe((event: {item: Item, data: any}) => {
            let eventData = event.data;
            let item = event.item;
            item.data.icon = eventData.icon;
            this._itemService.updateItem(item.id, item.data).subscribe(
                this.onUpdateItem.bind(this)
            );
        });

        this._itemActionService.registerAction('update_modifiers').subscribe((event: {item: Item, data: any}) => {
            let item = event.item;
            this._itemService.updateItemModifiers(item.id, item.modifiers).subscribe(
                this.onUpdateModifiers.bind(this)
            );
        });

        this._itemActionService.registerAction('update_data').subscribe((event: {item: Item, data: any}) => {
            let item = event.item;
            this._itemService.updateItem(item.id, item.data).subscribe(
                this.onUpdateItem.bind(this)
            );
        });
    }
}
