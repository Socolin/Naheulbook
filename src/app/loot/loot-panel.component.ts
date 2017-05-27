import {OnDestroy} from '@angular/core';
import {NotificationsService} from '../notifications/notifications.service';
import {Loot} from './loot.model';
import {LootWebsocketService} from './loot.websocket.service';
import {Subscription} from 'rxjs';
import {Item} from '../character/item.model';
import {CharacterResume} from '../character/character.model';

export class LootPanelComponent implements OnDestroy {
    public loots: Loot[] = [];
    public lootSubscriptions: {[lootId: number]: Subscription[]} = {};
    public subscriptions: Subscription[] = [];

    constructor(protected _lootWebsocketService: LootWebsocketService
        , protected _notification: NotificationsService) {
    }

    lootAdded(loot: Loot, noNotifications?: boolean): boolean {
        let i = this.loots.findIndex(l => l.id === loot.id);
        if (i !== -1) {
            return false;
        }
        this.loots.unshift(loot);
        this.registerLoot(loot, noNotifications);
        return true;
    }

    private registerLoot(loot: Loot, noNotifications: boolean) {
        this._lootWebsocketService.register(loot);
        let sub;
        this.lootSubscriptions[loot.id] = [];

        if (!noNotifications) {
            sub = loot.onTookItem.subscribe((change: { character: CharacterResume, item: Item }) => {
                let item = change.item;
                let character = change.character;
                if (item.data.quantity) {
                    this._notification.info('Loot', character.name + ' à pris '
                        + item.data.quantity + ' ' + item.data.name);
                } else {
                    this._notification.info('Loot', character.name + ' à pris ' + item.data.name);
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
        this._lootWebsocketService.unregister(loot);
        loot.dispose();
        this.loots.splice(i, 1);
        this.lootSubscriptions[loot.id].forEach(sub => sub.unsubscribe());
        delete this.lootSubscriptions[loot.id];
        return true;
    }

    ngOnDestroy(): void {
        for (let i = 0; i < this.loots.length; i++) {
            let loot = this.loots[i];
            this._lootWebsocketService.unregister(loot);
            loot.dispose();
        }
        for (let i = 0; i < this.subscriptions.length; i++) {
            this.subscriptions[i].unsubscribe();
        }
    }

    onLoadLoots(loots: Loot[], noNotifications?: boolean) {
        this.loots = loots;
        for (let loot of loots) {
            this.registerLoot(loot, noNotifications);
        }
    }
}
