import {forkJoin, Observable} from 'rxjs';

import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {ActiveStatsModifier} from '../shared';

import {Skill, SkillService} from '../skill';

import {Item, ItemData} from './item.model';
import {ItemPartialResponse} from '../api/responses';
import {assertNever} from '../utils/utils';
import {Guid} from '../api/shared/util';

export interface TakeItemResponse {
    takenItem: ItemData;
    remainingQuantity: number;
}

@Injectable()
export class ItemService {
    constructor(
        private readonly httpClient: HttpClient,
        private readonly skillService: SkillService,
    ) {
    }

    deleteItem(itemId: number): Observable<void> {
        return this.httpClient.delete<void>(`/api/v2/items/${itemId}`);
    }

    takeItemFromLoot(itemId: number, characterId: number, quantity?: number): Observable<TakeItemResponse> {
        return this.httpClient.post<TakeItemResponse>(`/api/v2/items/${itemId}/take`, {
            quantity: quantity,
            characterId: characterId
        });
    }

    giveItem(itemId: number, characterId: number, quantity?: number): Observable<{ remainingQuantity: number }> {
        return this.httpClient.post<{ remainingQuantity: number }>(`/api/v2/items/${itemId}/give`, {
            characterId: characterId,
            quantity: quantity
        });
    }

    addItemTo(
        targetType: 'character' | 'monster' | 'loot',
        targetId: number,
        itemTemplateId: Guid,
        itemData: ItemData,
    ): Observable<Item> {
        if (itemData.quantity) {
            itemData.quantity = +itemData.quantity;
        } else {
            delete itemData['quantity'];
        }

        return new Observable(observer => {
            let url: string;
            switch (targetType) {
                case 'character':
                    url = `/api/v2/characters/${targetId}/items`;
                    break;
                case 'monster':
                    url = `/api/v2/monsters/${targetId}/items`;
                    break;
                case 'loot':
                    url = `/api/v2/loots/${targetId}/items`;
                    break;
                default:
                    assertNever(targetType);
                    // FIXME: Remove next line when asserts is available (Typescript 3.7)
                    throw new Error('Waiting for asserts');
            }
            forkJoin([
                this.httpClient.post(url, {
                    itemTemplateId: itemTemplateId,
                    itemData: itemData
                }),
                this.skillService.getSkillsById()
            ]).subscribe(
                ([itemJsonData, skillsById]: [Item, { [skillId: number]: Skill }]) => {
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

    addRandomItemTo(
        targetType: 'character' | 'monster' | 'loot',
        targetId: number,
        subCategoryTechName: string,
    ): Observable<Item> {
        return new Observable(observer => {
            let url: string;
            switch (targetType) {
                case 'character':
                    url = `/api/v2/characters/${targetId}/addRandomItem`;
                    break;
                case 'monster':
                    url = `/api/v2/monsters/${targetId}/addRandomItem`;
                    break;
                case 'loot':
                    url = `/api/v2/loots/${targetId}/addRandomItem`;
                    break;
                default:
                    assertNever(targetType);
                    // FIXME: Remove next line when asserts is available (Typescript 3.7)
                    throw new Error('Waiting for asserts');
            }

            forkJoin([
                this.httpClient.post(url, {
                    subCategoryTechName
                }),
                this.skillService.getSkillsById()
            ]).subscribe(
                ([itemJsonData, skillsById]: [Item, { [skillId: number]: Skill }]) => {
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

    equipItem(itemId: number, level: number): Observable<ItemPartialResponse> {
        return this.httpClient.post<ItemPartialResponse>(`/api/v2/items/${itemId}/equip`, {
            level
        });
    }

    moveToContainer(itemId: number, containerId?: number): Observable<ItemPartialResponse> {
        return this.httpClient.put<ItemPartialResponse>(`/api/v2/items/${itemId}/container`, {
            containerId: containerId
        });
    }

    updateItem(itemId: number, itemData: ItemData): Observable<ItemPartialResponse> {
        return this.httpClient.put<ItemPartialResponse>(`/api/v2/items/${itemId}/data`, itemData);
    }

    useItemCharge(itemId: number): Observable<ItemPartialResponse> {
        return this.httpClient.post<ItemPartialResponse>(`/api/v2/items/${itemId}/useCharge`, {});
    }

    updateItemModifiers(itemId: number, modifiers: ActiveStatsModifier[]): Observable<ItemPartialResponse> {
        return this.httpClient.put<ItemPartialResponse>(`/api/v2/items/${itemId}/modifiers`, modifiers);
    }
}
