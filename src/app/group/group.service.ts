import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable} from 'rxjs/Rx';

import {HistoryEntry} from '../shared';
import {Monster} from '../monster';
import {JsonService} from '../shared/json-service';
import {NotificationsService} from '../notifications/notifications.service';
import {LoginService} from '../user/login.service';
import {Character, CharacterGiveDestination, CharacterResume} from '../character/character.model';
import {Group, GroupData, GroupJsonData} from './group.model';
import {NhbkDateOffset} from '../date/date.model';
import {Loot} from '../loot';
import {CharacterService} from '../character/character.service';
import {WebSocketService} from '../shared/websocket.service';
import {EventService} from '../event/event.service';
import {NEvent} from '../event/event.model';

@Injectable()
export class GroupService extends JsonService {
    constructor(http: Http
        , notification: NotificationsService
        , private _characterService: CharacterService
        , private _websocketService: WebSocketService
        , private _eventService: EventService
        , loginService: LoginService) {
        super(http, notification, loginService);
    }

    getGroup(groupId): Observable<Group> {
        return this.postJson('/api/character/groupDetail', {
            groupId: groupId
        }).map(res => res.json()).map((groupData: Group) => {
            let charactersLoading: Observable<Character>[] = [];
            for (let i = 0; i < groupData.characters.length; i++) {
                let char = groupData.characters[i];
                charactersLoading.push(this._characterService.getCharacter(char.id));
            }

            let group = Group.fromJson(groupData);
            this._websocketService.registerElement(group);

            Observable.forkJoin(
                Observable.forkJoin(charactersLoading),
                this.loadMonsters(group.id),
                this.loadLoots(group.id),
                this._eventService.loadEvents(group.id)
            ).subscribe(([characters, monsters, loots, events]: [Character[], Monster[], Loot[], NEvent[]]) => {
                for (let i = 0; i < characters.length; i++) {
                    let character = characters[i];
                    group.addCharacter(character);
                }
                for (let i = 0; i < monsters.length; i++) {
                    let monster = monsters[i];
                    group.addMonster(monster);
                }
                for (let i = 0; i < loots.length; i++) {
                    let loot = loots[i];
                    group.addLoot(loot);
                }
                for (let i = 0; i < events.length; i++) {
                    let event = events[i];
                    group.addEvent(event);
                }
                let charactersId: number[] = [];
                for (let i = 0; i < group.invited.length; i++) {
                    charactersId.push(group.invited[i].id);
                }
                for (let i = 0; i < group.invites.length; i++) {
                    charactersId.push(group.invites[i].id);
                }
                this._characterService.loadCharactersResume(charactersId).subscribe(
                    (characterInvite: CharacterResume[]) => {
                        for (let i = 0; i < group.invited.length; i++) {
                            let char = group.invited[i];
                            for (let j = 0; j < characterInvite.length; j++) {
                                let c = characterInvite[j];
                                if (c.id === char.id) {
                                    group.invited[i] = c;
                                }
                            }
                        }
                        for (let i = 0; i < group.invites.length; i++) {
                            let char = group.invites[i];
                            for (let j = 0; j < characterInvite.length; j++) {
                                let c = characterInvite[j];
                                if (c.id === char.id) {
                                    group.invites[i] = c;
                                }
                            }
                        }
                    }
                );
            });
            return group;
        });
    }

    createGroup(name): Observable<GroupJsonData> {
        return this.postJson('/api/character/createGroup', {
            name: name
        }).map(res => res.json());
    }

    /* Monster */

    loadMonsters(groupId: number): Observable<Monster[]> {
        return this.postJson('/api/group/loadMonsters', {
            groupId: groupId
        }).map(res => Monster.monstersFromJson(res.json()));
    }

    loadDeadMonsters(groupId: number, startIndex: number, count: number): Observable<Monster[]> {
        return this.postJson('/api/group/loadDeadMonsters', {
            groupId: groupId,
            startIndex: startIndex,
            count: count
        }).map(res => Monster.monstersFromJson(res.json()));
    }

    createMonster(groupId: number, monster): Observable<Monster> {
        return this.postJson('/api/group/createMonster', {
            monster: monster,
            groupId: groupId
        }).map(res => Monster.fromJson(res.json()));
    }

    updateMonster(monsterId: number, fieldName: string, value: any): Observable<{value: any, fieldName: string}> {
        return this.postJson('/api/group/updateMonster', {
            fieldName: fieldName,
            value: value,
            monsterId: monsterId
        }).map(res => res.json());
    }

    killMonster(monsterId: number): Observable<Monster> {
        return this.postJson('/api/group/killMonster', {
            monsterId: monsterId
        }).map(res => Monster.fromJson(res.json()));
    }

    deleteMonster(monsterId: number): Observable<any> {
        return this.postJson('/api/group/deleteMonster', {
            monsterId: monsterId
        }).map(res => res.json());
    }

    /* Loot */

    loadLoots(groupId: number): Observable<Loot[]> {
        return this.postJson('/api/group/loadLoots', {
            groupId: groupId
        }).map(res => Loot.lootsFromJson(res.json()));
    }

    createLoot(groupId: number, lootName: string): Observable<Loot> {
        return this.postJson('/api/group/createLoot', {
            groupId: groupId,
            name: lootName
        }).map(res => Loot.fromJson(res.json()));
    }

    deleteLoot(lootId: number) {
        return this.postJson('/api/group/deleteLoot', {
            lootId: lootId
        });
    }

    updateLoot(loot: Loot): Observable<Loot> {
        return this.postJson('/api/group/updateLoot', {
            loot: {
                visibleForPlayer: loot.visibleForPlayer,
                id: loot.id
            }
        }).map(res => Loot.fromJson(res.json()));
    }

    /* History */

    loadHistory(groupId: number, page: number): Observable<HistoryEntry[]> {
        return this.postJson('/api/group/history', {
            groupId: groupId,
            page: page
        }).map(res => res.json());
    }

    addLog(groupId: number, info: string, is_gm: boolean): Observable<Object> {
        return this.postJson('/api/group/addLog', {
            groupId: groupId,
            is_gm: is_gm,
            info: info
        }).map(res => res.json());
    }

    /* Misc */

    editGroupValue(groupId: number, key: string, value: any): Observable<GroupData> {
        return this.postJson('/api/group/edit', {
            groupId: groupId,
            key: key,
            value: value
        }).map(res => res.json());
    }

    addTime(groupId: number, dateOffset: NhbkDateOffset): Observable<GroupData> {
        return this.postJson('/api/group/addTime', {
            groupId: groupId,
            dateOffset: dateOffset
        }).map(res => res.json());
    }

    nextLap(groupId: number): Observable<GroupData> {
        return this.postJson('/api/group/nextLap', {
            groupId: groupId
        }).map(res => res.json());
    }

    listActiveCharactersInGroup(characterId: number): Observable<CharacterGiveDestination[]> {
        return this.postJson('/api/group/listActiveCharacters', {
            characterId: characterId
        }).map(res => res.json());
    }
}
