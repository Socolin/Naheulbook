import {Component, Input, OnInit} from '@angular/core';
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

    constructor(private lootWebsocketService: LootWebsocketService
        , private notification: NotificationsService
        , private _itemService: ItemService
        , private _characterService: CharacterService
        , private _characterWebsocketService: CharacterWebsocketService) {
        super(lootWebsocketService, notification);
    }

    takeItemFromLoot(loot: Loot, item: Item) {
        if (item != null) {
            this._itemService.takeItemFromLoot(item.id, this.character.id).subscribe(
                takenItem => {
                    this.onRemoveItemFromLoot(loot, takenItem);
                }
            );
        }
    }

    takeItemFromMonster(monster: Monster, item: Item) {
        if (item != null) {
            this._itemService.takeItemFromLoot(item.id, this.character.id).subscribe(
                takenItem => {
                    this.onRemoveItemFromMonster(monster, takenItem);
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
