import {Input, OnInit, Component, Inject, forwardRef, Output, EventEmitter} from "@angular/core";
import {Character} from "./character.model";
import {CharacterWebsocketService} from "./character-websocket.service";
import {ItemService} from "../item/item.service";
import {ItemTemplate} from "../item/item-template.model";
import {Item, PartialItem, ItemData} from "./item.model";
import {removeDiacritics} from "../shared/remove_diacritics";
import {SwipeService} from "./swipe.service";
import {ItemActionService} from "./item-action.service";



@Component({
    selector: 'inventory-panel',
    templateUrl: 'inventory-panel.component.html',
    providers: [SwipeService],
})
export class InventoryPanelComponent implements OnInit {
    @Input() character: Character;

    // Inventory
    public selectedItem: Item;

    public selectedInventoryTab: string = 'all';
    public filteredItems: ItemTemplate[] = [];
    public itemAddCustomName: string;
    public itemAddCustomDescription: string;
    public selectedAddItem: ItemTemplate;
    public itemAddQuantity: number;

    constructor(
        @Inject(forwardRef(()  => ItemService)) private _itemService: ItemService
        , private _characterWebsocketService: CharacterWebsocketService
        , private _itemActionService: ItemActionService
        , private _swipeService: SwipeService) {
    }

    updateFilterItem() {
        if (this.selectedInventoryTab === 'add') {
            this.updateFilterAddItem();
        }
    }

    updateFilterAddItem() {
        if (this.itemFilterName) {
            this._itemService.searchItem(this.itemFilterName).subscribe(
                items => {
                    this.filteredItems = items;
                    this.selectedAddItem = null;
                }
            );
        }
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

    unselectAddItem() {
        this.selectedAddItem = null;
        return false;
    }

    isAddItemSelected(item) {
        return this.selectedAddItem && this.selectedAddItem.id === item.id;
    }

    onAddItem(item: Item) {
        for (let i = 0; i < this.character.items.length; i++) {
            if (this.character.items[i].id === item.id) {
                return;
            }
        }

        this._characterWebsocketService.notifyChange("Ajout de l'objet: " + item.data.name);
        this.character.items.push(item);
        this.character.update();
    }

    addItem() {
        if (this.character) {
            let itemData = new ItemData();
            itemData['name'] = this.itemAddCustomName;
            itemData['description'] = this.itemAddCustomDescription;
            itemData['quantity'] = this.itemAddQuantity;
            this._itemService.addItem(this.character.id
                , this.selectedAddItem.id
                , itemData)
                .subscribe(
                    item => {
                        this.onAddItem(item);
                        this.selectedItem = item;
                        this.itemFilterName = "";
                        this.selectedAddItem = null;
                        this.itemAddCustomName = null;
                        this.itemAddCustomDescription = null;
                        this.itemAddQuantity = null;
                    }
                );
        }
    }

    selectItem(item: Item) {
        this.selectedItem = item;
        return false;
    }

    private itemFilterName: string;
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

    private sortType = 'none';
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
                this.character.items.sort((a, b) =>
                    {
                        return a.data.name.localeCompare(b.data.name);
                    }
                );
            }
            else {
                this.sortType = 'desc';
                this.character.items.sort((a, b) =>
                    {
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
        for (let i = 0; i < this.character.items.length; i++) {
            let it = this.character.items[i];
            if (it.id === item.id) {
                this.character.items.splice(i, 1);
                this.character.update();
                this._characterWebsocketService.notifyChange("Suppression de l'objet: " + item.data.name);
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
                this._characterWebsocketService.notifyChange("Utilisation d'une charge de l'objet: " + item.data.name);
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
                this._characterWebsocketService.notifyChange("Identification de l'objet: " + item.data.name);
                break;
            }
        }
    }

    onUpdateItemName(item: PartialItem) {
        for (let i = 0; i < this.character.items.length; i++) {
            let it = this.character.items[i];
            if (it.id === item.id) {
                it.data = item.data;
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
                    this._characterWebsocketService.notifyChange("Modification de la quantité de l'objet: " + item.data.name + ": " + item.data.quantity + " ->" + it.data.quantity);
                    it.data.quantity = item.data.quantity;
                    this.character.update();
                }
                break;
            }
        }
    }

    ngOnInit() {
        this._characterWebsocketService.registerPacket("equipItem").subscribe(this.onEquipItem.bind(this));
        this._characterWebsocketService.registerPacket("addItem").subscribe(this.onAddItem.bind(this));
        this._characterWebsocketService.registerPacket("deleteItem").subscribe(this.onDeleteItem.bind(this));
        this._characterWebsocketService.registerPacket("identifyItem").subscribe(this.onIdentifyItem.bind(this));
        this._characterWebsocketService.registerPacket("useCharge").subscribe(this.onUseItemCharge.bind(this));
        this._characterWebsocketService.registerPacket("changeContainer").subscribe(this.onChangeContainer.bind(this));
        this._characterWebsocketService.registerPacket("updateItemName").subscribe(this.onUpdateItemName.bind(this));
        this._characterWebsocketService.registerPacket("changeQuantity").subscribe(this.onUpdateQuantity.bind(this));

        this._itemActionService.registerAction('equip').subscribe((event: {item: Item, data: any})=> {
            let eventData = event.data;
            let item = event.item;
            let level = 1;
            if (eventData != null && eventData.level != null) {
                level = eventData.level;
            }
            this._itemService.equipItem(item.id, level).subscribe(
                this.onEquipItem.bind(this)
            );
        });
        this._itemActionService.registerAction('unequip').subscribe((event: {item: Item, data: any})=> {
            let item = event.item;
            this._itemService.equipItem(item.id, 0).subscribe(
                this.onEquipItem.bind(this)
            );
        });
        this._itemActionService.registerAction('delete').subscribe((event: {item: Item, data: any})=> {
            let item = event.item;
            this._itemService.deleteItem(item.id).subscribe(
                deletedItem => {
                    this.onDeleteItem(deletedItem);
                    this.selectedItem = null;
                }
            );
        });
        this._itemActionService.registerAction('update_quantity').subscribe((event: {item: Item, data: any})=> {
            let eventData = event.data;
            let item = event.item;
            this._itemService.updateQuantity(item.id, eventData.quantity).subscribe(
                res => {
                    this.onUpdateQuantity(res);
                }
            );
        });
        this._itemActionService.registerAction('read_skill_book').subscribe((event: {item: Item, data: any})=> {
            let item = event.item;
            this._itemService.readBook(item.id).subscribe(
                res => {
                    item.data.readCount = res.data.readCount;
                    this.character.update();
                }
            );
        });
        this._itemActionService.registerAction('identify').subscribe((event: {item: Item, data: any})=> {
            let item = event.item;
            this._itemService.identify(item.id).subscribe(
                this.onIdentifyItem.bind(this)
            );
        });
        this._itemActionService.registerAction('use_charge').subscribe((event: {item: Item, data: any})=> {
            let item = event.item;
            this._itemService.updateCharge(item.id, item.data.charge - 1).subscribe(
                this.onUseItemCharge.bind(this)
            );
        });
        this._itemActionService.registerAction('move_to_container').subscribe((event: {item: Item, data: any})=> {
            let eventData = event.data;
            let item = event.item;
            this._itemService.moveToContainer(item.id, eventData.container).subscribe(
                this.onChangeContainer.bind(this)
            );
        });

        this._itemActionService.registerAction('edit_item_name').subscribe((event: {item: Item, data: any})=> {
            let eventData = event.data;
            let item = event.item;
            let data = {
                name: eventData.name,
                description: eventData.description
            };
            this._itemService.updateItem(item.id, data).subscribe(
                this.onUpdateItemName.bind(this)
            );
        });
    }
}
