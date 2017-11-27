import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable} from 'rxjs/Rx';

import {CharacterSummary, JsonService, HistoryEntry} from '../shared';
import {NotificationsService} from '../notifications';
import {Monster} from '../monster';
import {LoginService} from '../user';
import {Loot} from '../loot';
import {NEvent, EventService} from '../event';
import {Character, CharacterService} from '../character';
import {NhbkDateOffset} from '../date';
import {Skill, SkillService} from '../skill';

import {CharacterInviteInfo, Group, GroupData, GroupJsonData, PartialGroup} from './group.model';

@Injectable()
export class GroupService extends JsonService {
    constructor(http: Http
        , notification: NotificationsService
        , private _characterService: CharacterService
        , private _eventService: EventService
        , private _skillService: SkillService
        , loginService: LoginService) {
        super(http, notification, loginService);
    }

    getGroup(groupId): Observable<Group> {
        return this.postJson('/api/character/groupDetail', {
            groupId: groupId
        }).map(res => res.json()).map((groupData: Group) => {
            let charactersLoading: Observable<Character|null>[] = [];
            for (let i = 0; i < groupData.characters.length; i++) {
                let char = groupData.characters[i];
                charactersLoading.push(this._characterService.getCharacter(char.id));
            }

            let group = Group.fromJson(groupData);

            if (charactersLoading.length === 0) {
                charactersLoading.push(Observable.of(null));
            }
            Observable.forkJoin(
                Observable.forkJoin(charactersLoading),
                this.loadMonsters(group.id),
                this.loadLoots(group.id),
                this._eventService.loadEvents(group.id)
            ).subscribe(([characters, monsters, loots, events]: [Character[], Monster[], Loot[], NEvent[]]) => {
                for (let character of characters) {
                    if (character === null) {
                        break;
                    }
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
                    (characterInvite: CharacterSummary[]) => {
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
                group.updateFightersOrder(true);
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
        return Observable.forkJoin(
            this.postJson('/api/monster/loadMonsters', {
                groupId: groupId
            }).map(res => res.json()),
            this._skillService.getSkillsById()
        ).map(([monstersJsonData, skillsById]: [any[], {[skillId: number]: Skill}]) => {
            return Monster.monstersFromJson(monstersJsonData, skillsById)
        });
    }

    loadDeadMonsters(groupId: number, startIndex: number, count: number): Observable<Monster[]> {
        return Observable.forkJoin(
            this.postJson('/api/monster/loadDeadMonsters', {
                groupId: groupId,
                startIndex: startIndex,
                count: count
            }).map(res => res.json()),
            this._skillService.getSkillsById()
        ).map(([monstersJsonData, skillsById]: [any[], {[skillId: number]: Skill}]) => {
            return Monster.monstersFromJson(monstersJsonData, skillsById)
        });
    }

    /* Loot */

    loadLoots(groupId: number): Observable<Loot[]> {
        return Observable.forkJoin(
            this.postJson('/api/group/loadLoots', {
                groupId: groupId
            }).map(res => res.json()),
            this._skillService.getSkillsById()
        ).map(([lootsJsonData, skillsById]: [any[], {[skillId: number]: Skill}]) => {
            return Loot.lootsFromJson(lootsJsonData, skillsById)
        });
    }

    createLoot(groupId: number, lootName: string): Observable<Loot> {
        return Observable.forkJoin(
            this.postJson('/api/group/createLoot', {
                groupId: groupId,
                name: lootName
            }).map(res => res.json()),
            this._skillService.getSkillsById()
        ).map(([lootJsonData, skillsById]: [any, {[skillId: number]: Skill}]) => {
            return Loot.fromJson(lootJsonData, skillsById)
        });
    }

    deleteLoot(lootId: number) {
        return this.postJson('/api/group/deleteLoot', {
            lootId: lootId
        });
    }

    updateLoot(loot: Loot): Observable<Loot> {
        return Observable.forkJoin(
            this.postJson('/api/group/updateLoot', {
                loot: {
                    visibleForPlayer: loot.visibleForPlayer,
                    id: loot.id
                }
            }).map(res => res.json()),
            this._skillService.getSkillsById()
        ).map(([lootJsonData, skillsById]: [any, {[skillId: number]: Skill}]) => {
            return Loot.fromJson(lootJsonData, skillsById)
        });
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

    nextFighter(groupId: number, fighterIndex: number) {
        return this.postJson('/api/group/nextFighter', {
            groupId: groupId,
            fighterIndex: fighterIndex,
        }).map(res => res.json());
    }

    saveChangedTime(groupId: number, changes: any[]): Observable<any> {
        return this.postJson('/api/group/saveChangedTime', {
            groupId: groupId,
            modifiersDurationUpdated: changes,
        }).map(res => res.json());
    }

    searchPlayersForInvite(name: string, groupId: number): Observable<CharacterInviteInfo[]> {
        return this.postJson('/api/character/searchForInvite', {
            name: name,
            groupId: groupId
        }).map(res => res.json());
    }

    inviteCharacter(groupId: number, characterId: number): Observable<CharacterInviteInfo> {
        return this.postJson('/api/character/invite', {
            characterId: characterId,
            groupId: groupId,
            fromGroup: true
        }).map(res => res.json());
    }

    listGroups(): Observable<PartialGroup[]> {
        return this._http.get('/api/character/listGroups').map(res => res.json());
    }
}
