import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {OverlayRef, Portal} from '@angular/material';

import {NotificationsService} from '../notifications/notifications.service';
import {LootPanelComponent} from '../loot/loot-panel.component';
import {Character} from './character.model';
import {Loot} from '../loot/loot.model';
import {Item} from './item.model';
import {Monster} from '../monster/monster.model';
import {ItemService} from './item.service';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';

@Component({
    selector: 'character-loot-panel',
    styleUrls: ['./character-loot-panel.component.scss'],
    templateUrl: './character-loot-panel.component.html',
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

    constructor(private notification: NotificationsService
        , private _itemService: ItemService
        , private _nhbkDialogservice: NhbkDialogService) {
        super(notification);
    }

    openTakeItemLootDialog(loot: Loot, item: Item) {
        this.takingItem = item;
        this.takingQuantity = item.data.quantity;
        this.takingItemLoot = loot;
        this.takingItemMonster = null;
        this.takeItemOverlayRef = this._nhbkDialogservice.openCenteredBackdropDialog(this.takeItemDialog);
    }

    openTakeItemMonsterDialog(monster: Monster, item: Item) {
        this.takingItem = item;
        this.takingQuantity = item.data.quantity;
        this.takingItemLoot = null;
        this.takingItemMonster = monster;
        this.takeItemOverlayRef = this._nhbkDialogservice.openCenteredBackdropDialog(this.takeItemDialog);
    }

    closeTakeItemDialog() {
        this.takeItemOverlayRef.detach();
    }

    validTakeItem() {
        if (this.takingItemMonster) {
            this.takeItemFromMonster(this.takingItemMonster, this.takingItem, this.takingQuantity);
        } else if (this.takingItemLoot) {
            this.takeItemFromLoot(this.takingItemLoot, this.takingItem, this.takingQuantity);
        }
        this.closeTakeItemDialog();
    }

    takeItemFromLoot(loot: Loot, item: Item, quantity?: number) {
        if (item != null) {
            this._itemService.takeItemFromLoot(item.id, this.character.id, quantity).subscribe(
                takenItemData => {
                    loot.takeItem(takenItemData.leftItem, takenItemData.item, takenItemData.character);
                }
            );
        }
    }

    takeItemFromMonster(monster: Monster, item: Item, quantity?: number) {
        if (item != null) {
            this._itemService.takeItemFromLoot(item.id, this.character.id, quantity).subscribe(
                takenItemData => {
                    monster.takeItem(takenItemData.leftItem, takenItemData.item, takenItemData.character);
                }
            );
        }
    }

    selectItem(item: Item): boolean {
        this.selectedItem = item;
        return false;
    }

    ngOnInit(): void {
        this.loots = this.character.loots;
    }
}
