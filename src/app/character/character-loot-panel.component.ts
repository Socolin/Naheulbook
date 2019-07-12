import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {NhbkDialogService} from '../shared';
import {NotificationsService} from '../notifications';
import {Loot, LootPanelComponent} from '../loot';
import {Monster} from '../monster';

import {Character} from './character.model';
import {Item, ItemService} from '../item';

@Component({
    selector: 'character-loot-panel',
    styleUrls: ['./character-loot-panel.component.scss'],
    templateUrl: './character-loot-panel.component.html',
})
export class CharacterLootPanelComponent extends LootPanelComponent implements OnInit {
    @Input() character: Character;
    @Input() inGroupTab: boolean;
    public selectedItem?: Item;

    @ViewChild('takeItemDialog', {static: true})
    public takeItemDialog: Portal<any>;
    public takeItemOverlayRef: OverlayRef;
    public takingItemLoot: Loot | undefined;
    public takingItemMonster: Monster | undefined;
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
        this.takingItemMonster = undefined;
        this.takeItemOverlayRef = this._nhbkDialogservice.openCenteredBackdropDialog(this.takeItemDialog);
    }

    openTakeItemMonsterDialog(monster: Monster, item: Item) {
        this.takingItem = item;
        this.takingQuantity = item.data.quantity;
        this.takingItemLoot = undefined;
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
                    loot.takeItem(item.id, takenItemData.remainingQuantity, this.character);
                }
            );
        }
    }

    takeItemFromMonster(monster: Monster, item: Item, quantity?: number) {
        if (item != null) {
            this._itemService.takeItemFromLoot(item.id, this.character.id, quantity).subscribe(
                takenItemData => {
                    monster.takeItem(item.id, takenItemData.remainingQuantity, this.character);
                }
            );
        }
    }

    deselectItem(): void {
        this.selectedItem = undefined;
    }

    selectItem(item: Item): boolean {
        this.selectedItem = item;
        return false;
    }

    ngOnInit(): void {
        this.loots = this.character.loots;
    }
}
