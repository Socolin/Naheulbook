import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {OverlayRef, Portal} from '@angular/material';

import {NotificationsService} from '../notifications/notifications.service';
import {LootWebsocketService} from '../loot/loot.websocket.service';
import {LootPanelComponent} from '../loot/loot-panel.component';
import {CharacterWebsocketService} from './character-websocket.service';
import {CharacterService} from './character.service';
import {Character} from './character.model';
import {Loot} from '../loot/loot.model';
import {Item} from './item.model';
import {Monster} from '../monster/monster.model';
import {ItemService} from '../item/item.service';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';

@Component({
    selector: 'character-loot-panel',
    styleUrls: ['./character-loot-panel.component.scss'],
    templateUrl: './character-loot-panel.component.html',
    providers: [LootWebsocketService],
})
export class CharacterLootPanelComponent extends LootPanelComponent implements OnInit {
    @Input() character: Character;
    @Input() inGroupTab: boolean;
    public selectedItem: Item;

    @ViewChild('takeItemDialog')
    public takeItemDialog: Portal<any>;
    public takeItemOverlayRef: OverlayRef;
    public takingItemLoot: Loot;
    public takingItemMonster: Monster;
    public takingItem: Item;
    public takingQuantity?: number;

    constructor(private lootWebsocketService: LootWebsocketService
        , private notification: NotificationsService
        , private _itemService: ItemService
        , private _characterService: CharacterService
        , private _nhbkDialogservice: NhbkDialogService
        , private _characterWebsocketService: CharacterWebsocketService) {
        super(lootWebsocketService, notification);
    }

    openTakeItemLootDialog(loot: Loot, item: Item) {
        this.takingItem = item;
        this.takingQuantity = item.data.quantity;
        this.takingItemLoot = loot;
        this.takeItemOverlayRef = this._nhbkDialogservice.openCenteredBackdropDialog(this.takeItemDialog);
    }

    openTakeItemMonsterDialog(monster: Monster, item: Item) {
        this.takingItem = item;
        this.takingQuantity = item.data.quantity;
        this.takingItemMonster = monster;
        this.takeItemOverlayRef = this._nhbkDialogservice.openCenteredBackdropDialog(this.takeItemDialog);
    }

    closeTakeItemDialog() {
        this.takeItemOverlayRef.detach();
    }

    validTakeItem() {
        if (this.takingItemLoot) {
            this.takeItemFromLoot(this.takingItemLoot, this.takingItem, this.takingQuantity);
        }
        else if (this.takingItemMonster) {
            this.takeItemFromMonster(this.takingItemMonster, this.takingItem, this.takingQuantity);
        }
        this.closeTakeItemDialog();
    }

    takeItemFromLoot(loot: Loot, item: Item, quantity?: number) {
        if (item != null) {
            this._itemService.takeItemFromLoot(item.id, this.character.id, quantity).subscribe(
                takenItem => {
                    this.onRemoveItemFromLoot(loot, takenItem, quantity);
                }
            );
        }
    }

    takeItemFromMonster(monster: Monster, item: Item, quantity?: number) {
        if (item != null) {
            this._itemService.takeItemFromLoot(item.id, this.character.id, quantity).subscribe(
                takenItem => {
                    this.onRemoveItemFromMonster(monster, takenItem, quantity);
                }
            );
        }
    }

    selectItem(item: Item): boolean {
        this.selectedItem = item;
        return false;
    }

    registerActions() {
        this._characterWebsocketService.registerPacket('showLoot').subscribe(this.onAddLoot.bind(this));
        this._characterWebsocketService.registerPacket('hideLoot').subscribe(this.onDeleteLoot.bind(this));
    }

    ngOnInit(): void {
        this._characterService.loadLoots(this.character.id).subscribe(
            loots => {
                this.onLoadLoots(loots);
                this.registerActions();
                if (this.inGroupTab) {
                    this._lootWebsocketService.registerNotifyFunction(null);
                }
            }
        );
    }
}
