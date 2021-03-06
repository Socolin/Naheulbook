import {Component, OnDestroy} from '@angular/core';
import {Subscription} from 'rxjs';

import {NotificationsService} from '../notifications';
import {Item} from '../item';

import {Loot} from './loot.model';
import {WebSocketService} from '../websocket';
import {CharacterSummaryResponse} from '../api/responses';

@Component({
    template: ``
})
export class LootPanelComponent implements OnDestroy {
    public loots: Loot[] = [];
    public lootSubscriptions: { [lootId: number]: Subscription[] } = {};
    public subscriptions: Subscription[] = [];

    constructor(
        protected readonly notification: NotificationsService,
        protected readonly webSocketService: WebSocketService
    ) {
    }

    lootAdded(loot: Loot, noNotifications: boolean): boolean {
        let i = this.loots.findIndex(l => l.id === loot.id);
        if (i !== -1) {
            return false;
        }
        this.loots.unshift(loot);
        this.registerLoot(loot, noNotifications);
        this.webSocketService.registerElement(loot);
        return true;
    }

    private registerLoot(loot: Loot, noNotifications: boolean) {
        let sub;
        this.lootSubscriptions[loot.id] = [];

        if (!noNotifications) {
            sub = loot.onTookItem.subscribe((change: { character: CharacterSummaryResponse, item: Item }) => {
                let item = change.item;
                let character = change.character;
                if (item.data.quantity) {
                    this.notification.info('Loot', character.name + ' à pris '
                        + item.data.quantity + ' ' + item.data.name);
                } else {
                    this.notification.info('Loot', character.name + ' à pris ' + item.data.name);
                }
            });
            this.lootSubscriptions[loot.id].push(sub);
        }
    }

    lootDeleted(lootId: number): boolean {
        let i = this.loots.findIndex(l => l.id === lootId);
        if (i === -1) {
            return false;
        }
        let loot = this.loots[i];
        loot.dispose();
        this.loots.splice(i, 1);
        if (this.lootSubscriptions[loot.id]) {
            this.lootSubscriptions[loot.id].forEach(sub => sub.unsubscribe());
            delete this.lootSubscriptions[loot.id];
        }
        return true;
    }

    ngOnDestroy(): void {
        for (let i = 0; i < this.loots.length; i++) {
            let loot = this.loots[i];
            loot.dispose();
        }
        for (let i = 0; i < this.subscriptions.length; i++) {
            this.subscriptions[i].unsubscribe();
        }
    }
}
