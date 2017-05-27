import {Injectable} from '@angular/core';
import {NotificationsService} from '../notifications/notifications.service';
import {WebSocketService} from '../shared/websocket.service';
import {GenericWebsocketService} from '../shared/generic.websocket.service';
import {MiscService} from '../shared/misc.service';
import {Loot} from './loot.model';
import {Subscription} from 'rxjs/Subscription';
import {Monster} from '../monster/monster.model';

@Injectable()
export class LootWebsocketService {
    constructor(private _notification: NotificationsService
        , private _miscService: MiscService
        , private _webSocketService: WebSocketService) {
    }

    register(loot: Loot): void {
        if (loot.wsSubscribtion) {
            throw new Error('Loot is already subscribed to websocket');
        }

        for (let monster of loot.monsters) {
            this.registerMonster(monster);
        }
        loot.monsterAdded.subscribe((monster: Monster) => {
            this.registerMonster(monster);
        });
        loot.monsterRemoved.subscribe((monster: Monster) => {
            this.unregisterMonster(monster);
        });

        loot.wsSubscribtion = this._webSocketService.register('loot', loot.id).subscribe(
            res => {
                try {
                    loot.onWebsocketData(res.opcode, res.data);
                } catch (err) {
                    this._notification.error('Erreur', 'Erreur WS');
                    this._miscService.postJson('/api/debug/report', err, true).subscribe();
                    console.error(err);
                }
            }
        );

    }

    registerMonster(monster: Monster): void {
        if (monster.wsSubscribtion) {
            throw new Error('Monster is already subscribed to websocket');
        }
        monster.wsSubscribtion = this._webSocketService.register('monster', monster.id).subscribe(
            res => {
                try {
                    monster.onWebsocketData(res.opcode, res.data);
                } catch (err) {
                    this._notification.error('Erreur', 'Erreur WS');
                    this._miscService.postJson('/api/debug/report', err, true).subscribe();
                    console.error(err);
                }
            }
        );
    }

    unregisterMonster(monster: Monster): void {
        if (monster.wsSubscribtion) {
            this._webSocketService.unregister('monster', monster.id);
            monster.wsSubscribtion.unsubscribe();
            monster.wsSubscribtion = null;
        }
    }

    unregister(loot: Loot): void {
        if (loot.wsSubscribtion) {
            this._webSocketService.unregister('loot', loot.id);
            loot.wsSubscribtion.unsubscribe();
            loot.wsSubscribtion = null;
        }
        for (let monster of loot.monsters) {
            this.unregisterMonster(monster);
        }
    }
}
