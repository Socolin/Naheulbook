import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable} from 'rxjs/Rx';

import {ActiveStatsModifier, JsonService} from '../shared';
import {NotificationsService} from '../notifications';

import {LoginService} from '../user';
import {Skill, SkillService} from '../skill';
import {LootTookItemMsg} from '../loot';

import {ItemData, PartialItem, Item} from './item.model';

@Injectable()
export class ItemService extends JsonService {
    constructor(http: Http
        , notification: NotificationsService
        , loginService: LoginService
        , private _skillService: SkillService) {
        super(http, notification, loginService);
    }

    deleteItem(itemId: number): Observable<Item> {
        return this.postJson('/api/item/delete', {itemId: itemId})
            .map(res => res.json());
    }

    takeItemFromLoot(itemId: number, characterId: number, quantity?: number): Observable<LootTookItemMsg> {
        return this.postJson('/api/item/takeItemFromLoot', {
            itemId: itemId,
            quantity: quantity,
            characterId: characterId
        }).map(res => res.json());
    }

    giveItem(itemId: number, characterId: number, quantity?: number): Observable<Item> {
        return this.postJson('/api/item/giveItem', {itemId: itemId, characterId: characterId, quantity: quantity})
            .map(res => res.json());
    }

    addItemTo(targetType: string
        , targetId: number
        , itemTemplateId: number
        , itemData: ItemData): Observable<Item> {
        if (itemData.quantity) {
            itemData.quantity = +itemData.quantity;
        } else {
            delete itemData['quantity'];
        }

        return Observable.create(observer => {
            Observable.forkJoin(this.postJson('/api/item/add', {
                    itemTemplateId: itemTemplateId,
                    targetId: targetId,
                    targetType: targetType,
                    itemData: itemData
                }).map(res => res.json()),
                this._skillService.getSkillsById()
            ).subscribe(
                ([itemJsonData, skillsById]: [Item, {[skillId: number]: Skill}]) => {
                    let item = Item.fromJson(itemJsonData, skillsById);
                    observer.next(item);
                    observer.complete();
                },
                err => {
                    observer.error(err);
                    observer.complete();
                }
            );
        });
    }
    addRandomItemTo(targetType: string
        , targetId: number
        , criteria: any): Observable<Item> {

        return Observable.create(observer => {
            Observable.forkJoin(this.postJson('/api/item/addRandom', {
                    targetId: targetId,
                    targetType: targetType,
                    criteria: criteria
                }).map(res => res.json()),
                this._skillService.getSkillsById()
            ).subscribe(
                ([itemJsonData, skillsById]: [Item, {[skillId: number]: Skill}]) => {
                    let item = Item.fromJson(itemJsonData, skillsById);
                    observer.next(item);
                    observer.complete();
                },
                err => {
                    observer.error(err);
                    observer.complete();
                }
            );
        });
    }

    equipItem(itemId: number, level: number): Observable<PartialItem> {
        return this.postJson('/api/item/equip', {
            itemId: itemId,
            level: level
        }).map(res => res.json());
    }

    moveToContainer(itemId: number, containerId: number): Observable<PartialItem> {
        return this.postJson('/api/item/moveToContainer', {
            itemId: itemId,
            containerId: containerId
        }).map(res => res.json());
    }

    updateQuantity(itemId: number, quantity): Observable<PartialItem> {
        return this.postJson('/api/item/updateQuantity', {
            itemId: itemId,
            quantity: quantity
        }).map(res => res.json());
    }

    readBook(itemId: number): Observable<PartialItem> {
        return this.postJson('/api/item/readBook', {
            itemId: itemId,
        }).map(res => res.json());
    }

    identify(itemId: number): Observable<PartialItem> {
        return this.postJson('/api/item/identify', {
            itemId: itemId,
        }).map(res => res.json());
    }

    updateCharge(itemId: number, charge): Observable<PartialItem> {
        return this.postJson('/api/item/updateCharge', {
            itemId: itemId,
            charge: charge
        }).map(res => res.json());
    }

    updateItem(itemId: number, itemData): Observable<PartialItem> {
        return this.postJson('/api/item/updateItem', {
            itemId: itemId,
            itemData: itemData
        }).map(res => res.json());
    }

    updateItemModifiers(itemId: number, modifiers: ActiveStatsModifier[]): Observable<PartialItem> {
        return this.postJson('/api/item/updateItemModifiers', {
            itemId: itemId,
            modifiers: modifiers
        }).map(res => res.json());
    }
}
