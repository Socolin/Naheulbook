import {Component, Input, OnInit} from '@angular/core';
import {NotificationsService} from '../notifications';
import {Loot, LootPanelComponent} from '../loot';
import {Monster} from '../monster';

import {Character} from './character.model';
import {Item, ItemService} from '../item';
import {WebSocketService} from '../websocket';
import {TakeLootDialogComponent, TakeLootDialogData, TakeLootDialogResult} from './take-loot-dialog.component';
import {NhbkMatDialog} from '../material-workaround';
import { MatCard, MatCardHeader, MatCardTitle, MatCardContent } from '@angular/material/card';
import { ItemLineComponent } from './item-line.component';
import { MatMenu, MatMenuContent, MatMenuItem } from '@angular/material/menu';
import { MatIcon } from '@angular/material/icon';

@Component({
    selector: 'character-loot-panel',
    styleUrls: ['./character-loot-panel.component.scss'],
    templateUrl: './character-loot-panel.component.html',
    imports: [MatCard, MatCardHeader, MatCardTitle, MatCardContent, ItemLineComponent, MatMenu, MatMenuContent, MatMenuItem, MatIcon]
})
export class CharacterLootPanelComponent extends LootPanelComponent implements OnInit {
    @Input() character: Character;
    @Input() inGroupTab: boolean;

    constructor(
        notification: NotificationsService,
        websocketService: WebSocketService,
        private readonly dialog: NhbkMatDialog,
        private readonly itemService: ItemService
    ) {
        super(notification, websocketService);
    }

    openTakeItemLootDialog(loot: Loot, item: Item) {
        const dialogRef = this.dialog.open<TakeLootDialogComponent, TakeLootDialogData, TakeLootDialogResult>(
            TakeLootDialogComponent,
            {
                autoFocus: false,
                data: {
                    item
                }
            }
        );

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            this.takeItemFromLoot(loot, item, result.quantity);
        });
    }

    openTakeItemMonsterDialog(monster: Monster, item: Item) {
        const dialogRef = this.dialog.open<TakeLootDialogComponent, TakeLootDialogData, TakeLootDialogResult>(
            TakeLootDialogComponent,
            {
                autoFocus: false,
                data: {
                    item
                }
            }
        );

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            this.takeItemFromMonster(monster, item, result.quantity);
        });
    }

    takeItemFromLoot(loot: Loot, item: Item, quantity?: number) {
        if (item != null) {
            this.itemService.takeItemFromLoot(item.id, this.character.id, quantity).subscribe(
                takenItemData => {
                    loot.takeItem(item.id, takenItemData.remainingQuantity, this.character);
                }
            );
        }
    }

    takeItemFromMonster(monster: Monster, item: Item, quantity?: number) {
        if (item != null) {
            this.itemService.takeItemFromLoot(item.id, this.character.id, quantity).subscribe(
                takenItemData => {
                    monster.takeItem(item.id, takenItemData.remainingQuantity, this.character);
                }
            );
        }
    }

    ngOnInit(): void {
        this.loots = this.character.loots;
    }
}
