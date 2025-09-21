import {Component, Input, OnDestroy, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications';

import {Item, ItemService} from '../item';
import {Loot, LootPanelComponent} from '../loot';
import {Monster} from '../monster';

import {Group} from './group.model';
import {GroupService} from './group.service';
import {openCreateItemDialog} from './create-item-dialog.component';
import {AddLootDialogComponent} from './add-loot-dialog.component';
import {WebSocketService} from '../websocket';
import {openCreateGemDialog} from './create-gem-dialog.component';
import {ItemDialogComponent} from '../item/item-dialog.component';
import {NhbkMatDialog} from '../material-workaround';
import {CommandSuggestionType, QuickAction, QuickCommandService} from '../quick-command';
import {Router} from '@angular/router';
import { MatButton, MatIconButton } from '@angular/material/button';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIcon } from '@angular/material/icon';
import { MatTooltip } from '@angular/material/tooltip';
import { MatMenuTrigger, MatMenu, MatMenuItem } from '@angular/material/menu';
import { MatCard, MatCardContent } from '@angular/material/card';
import { ItemListComponent } from '../item/item-list.component';

@Component({
    selector: 'group-loot-panel',
    styleUrls: ['./group-loot-panel.component.scss'],
    templateUrl: './group-loot-panel.component.html',
    imports: [MatButton, MatToolbar, MatIcon, MatTooltip, MatIconButton, MatMenuTrigger, MatMenu, MatMenuItem, MatCard, MatCardContent, ItemListComponent]
})
export class GroupLootPanelComponent extends LootPanelComponent implements OnInit, OnDestroy {
    @Input() group: Group;

    constructor(
        notification: NotificationsService,
        websocketService: WebSocketService,
        private readonly groupService: GroupService,
        private readonly itemService: ItemService,
        private readonly router: Router,
        private readonly quickCommandService: QuickCommandService,
        private readonly dialog: NhbkMatDialog,
    ) {
        super(notification, websocketService);
    }

    openAddItemDialog(target: Loot | Monster) {
        openCreateItemDialog(this.dialog, (item) => {
            if (target instanceof Loot) {
                this.onAddItem({loot: target, item: item});
            } else {
                this.onAddItem({monster: target, item: item});
            }
        });
    }

    openAddGemDialog(target: Loot | Monster) {
        openCreateGemDialog(this.dialog, (item) => {
            if (target instanceof Loot) {
                this.onAddItem({loot: target, item: item});
            } else {
                this.onAddItem({monster: target, item: item});
            }
        });
    }

    openAddLootDialog() {
        const dialogRef = this.dialog.open(AddLootDialogComponent);
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            this.groupService.createLoot(this.group.id, result).subscribe(
                loot => {
                    this.lootAdded(loot, false);
                }
            );
        })
    }

    deleteLoot(loot: Loot) {
        this.groupService.deleteLoot(loot.id).subscribe(
            () => {
                this.lootDeleted(loot.id);
            }
        );
    }

    onAddItem(data: { loot: Loot, item: Item } | { monster: Monster, item: Item }) {
        if ('loot' in data) {
            const loot = data.loot;
            this.itemService.addItemTo('loot', data.loot.id, data.item.template.id, data.item.data).subscribe(
                item => {
                    loot.addItem(item);
                }
            );
        } else {
            const monster = data.monster;
            this.itemService.addItemTo('monster', data.monster.id, data.item.template.id, data.item.data).subscribe(
                item => {
                    monster.addItem(item);
                }
            );
        }
    }

    openLoot(loot: Loot) {
        this.groupService.updateLootVisibility(loot.id, true).subscribe(
            () => {
                loot.visibleForPlayer = true;
            }
        );
    }

    closeLoot(loot: Loot) {
        this.groupService.updateLootVisibility(loot.id, false).subscribe(
            () => {
                loot.visibleForPlayer = false;
            }
        );
    }

    addRandomItemFromSubCategoryToLoot(loot: Loot, subCategoryName: string) {
        this.itemService.addRandomItemTo('loot', loot.id, subCategoryName).subscribe(
            item => {
                loot.addItem(item);
            }
        );
        return false;
    }

    addRandomItemFromSubCategoryToMonster(monster: Monster, subCategoryName: string) {
        this.itemService.addRandomItemTo('monster', monster.id, subCategoryName).subscribe(
            item => {
                monster.addItem(item);
            }
        );
        return false;
    }

    removeItemsFromLoot(loot: Loot, items: Item[]) {
        for (const item of items) {
            this.itemService.deleteItem(item.id).subscribe(
                () => {
                    loot.removeItem(item.id);
                }
            );
        }
    }

    removeItemsFromMonster(monster: Monster, items: Item[]) {
        for (const item of items) {
            this.itemService.deleteItem(item.id).subscribe(
                () => {
                    monster.removeItem(item.id);
                }
            );
        }
    }

    openItemDialog(item: Item) {
        this.dialog.open(ItemDialogComponent, {
            data: {item},
            autoFocus: false
        });
    }

    ngOnInit() {
        this.loots = this.group.loots;
        this.registerQuickActions();
    }

    ngOnDestroy() {
        super.ngOnDestroy();
        this.quickCommandService.unregisterActions('group-loot');
    }

    private registerQuickActions() {
        const actions: QuickAction[] = [];

        actions.push({
            type: CommandSuggestionType.Action,
            icon: 'add',
            priority: 40,
            displayText: 'Crée un loot',
            canBeUsedInRecent: true,
            action: () => {
                this.openAddLootDialog();
                this.router.navigate([], {fragment: 'loot'})
            },
        })

        this.quickCommandService.registerActions('group-loot', actions);
    }
}
