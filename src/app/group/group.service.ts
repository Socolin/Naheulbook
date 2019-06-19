
import {forkJoin, of as observableOf, Observable, Subject} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {CharacterSummary, HistoryEntry} from '../shared';
import {Monster} from '../monster';
import {Loot} from '../loot';
import {NEvent, EventService} from '../event';
import {Character, CharacterService} from '../character';
import {NhbkDateOffset} from '../date';
import {Skill, SkillService} from '../skill';

import {CharacterInviteInfo, Group, GroupData, GroupJsonData, PartialGroup} from './group.model';

@Injectable()
export class GroupService {
    constructor(private httpClient: HttpClient
        , private _characterService: CharacterService
        , private _eventService: EventService
        , private _skillService: SkillService) {
    }

    getGroup(groupId): Observable<Group> {
        let subject = new Subject<Group>();

        this.httpClient.post<Group>('/api/character/groupDetail', {
            groupId: groupId
        }).subscribe((groupData: Group) => {
            let charactersLoading: Observable<Character|null>[] = [];
            for (let i = 0; i < groupData.characters.length; i++) {
                let char = groupData.characters[i];
                charactersLoading.push(this._characterService.getCharacter(char.id));
            }

            let group = Group.fromJson(groupData);

            if (charactersLoading.length === 0) {
                charactersLoading.push(observableOf(null));
            }
            forkJoin([
                forkJoin(charactersLoading),
                this.loadMonsters(group.id),
                this.loadLoots(group.id),
                this._eventService.loadEvents(group.id)
            ]).subscribe(([characters, monsters, loots, events]: [Character[], Monster[], Loot[], NEvent[]]) => {
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
                subject.next(group);
                subject.complete();
            });
        });
        return subject;
    }

    createGroup(name): Observable<GroupJsonData> {
        return this.httpClient.post<GroupJsonData>('/api/character/createGroup', {
            name: name
        });
    }

    /* Monster */

    loadMonsters(groupId: number): Observable<Monster[]> {
        return forkJoin([
            this.httpClient.post<Monster[]>('/api/monster/loadMonsters', {
                groupId: groupId
            }),
            this._skillService.getSkillsById()
        ]).pipe(map(([monstersJsonData, skillsById]: [any[], {[skillId: number]: Skill}]) => {
            return Monster.monstersFromJson(monstersJsonData, skillsById)
        }));
    }

    loadDeadMonsters(groupId: number, startIndex: number, count: number): Observable<Monster[]> {
        return forkJoin([
            this.httpClient.post<Monster[]>('/api/monster/loadDeadMonsters', {
                groupId: groupId,
                startIndex: startIndex,
                count: count
            }),
            this._skillService.getSkillsById()
        ]).pipe(map(([monstersJsonData, skillsById]: [any[], {[skillId: number]: Skill}]) => {
            return Monster.monstersFromJson(monstersJsonData, skillsById)
        }));
    }

    /* Loot */

    loadLoots(groupId: number): Observable<Loot[]> {
        return forkJoin([
            this.httpClient.post<Loot[]>('/api/group/loadLoots', {
                groupId: groupId
            }),
            this._skillService.getSkillsById()
        ]).pipe(map(([lootsJsonData, skillsById]: [any[], {[skillId: number]: Skill}]) => {
            return Loot.lootsFromJson(lootsJsonData, skillsById)
        }));
    }

    createLoot(groupId: number, lootName: string): Observable<Loot> {
        return forkJoin([
            this.httpClient.post<Loot>('/api/group/createLoot', {
                groupId: groupId,
                name: lootName
            }),
            this._skillService.getSkillsById()
        ]).pipe(map(([lootJsonData, skillsById]: [any, {[skillId: number]: Skill}]) => {
            return Loot.fromJson(lootJsonData, skillsById)
        }));
    }

    deleteLoot(lootId: number) {
        return this.httpClient.post('/api/group/deleteLoot', {
            lootId: lootId
        });
    }

    updateLoot(loot: Loot): Observable<Loot> {
        return forkJoin([
            this.httpClient.post<Loot>('/api/group/updateLoot', {
                loot: {
                    visibleForPlayer: loot.visibleForPlayer,
                    id: loot.id
                }
            }),
            this._skillService.getSkillsById()
        ]).pipe(map(([lootJsonData, skillsById]: [any, {[skillId: number]: Skill}]) => {
            return Loot.fromJson(lootJsonData, skillsById)
        }));
    }

    /* History */

    loadHistory(groupId: number, page: number): Observable<HistoryEntry[]> {
        return this.httpClient.post<HistoryEntry[]>('/api/group/history', {
            groupId: groupId,
            page: page
        });
    }

    addLog(groupId: number, info: string, is_gm: boolean): Observable<Object> {
        return this.httpClient.post<Object>('/api/group/addLog', {
            groupId: groupId,
            is_gm: is_gm,
            info: info
        });
    }

    /* Misc */

    editGroupValue(groupId: number, key: string, value: any): Observable<GroupData> {
        return this.httpClient.post<GroupData>('/api/group/edit', {
            groupId: groupId,
            key: key,
            value: value
        });
    }

    addTime(groupId: number, dateOffset: NhbkDateOffset): Observable<GroupData> {
        return this.httpClient.post<GroupData>('/api/group/addTime', {
            groupId: groupId,
            dateOffset: dateOffset
        });
    }

    nextFighter(groupId: number, fighterIndex: number) {
        return this.httpClient.post('/api/group/nextFighter', {
            groupId: groupId,
            fighterIndex: fighterIndex,
        });
    }

    saveChangedTime(groupId: number, changes: any[]): Observable<any> {
        return this.httpClient.post<any>('/api/group/saveChangedTime', {
            groupId: groupId,
            modifiersDurationUpdated: changes,
        });
    }

    searchPlayersForInvite(name: string, groupId: number): Observable<CharacterInviteInfo[]> {
        return this.httpClient.post<CharacterInviteInfo[]>('/api/character/searchForInvite', {
            name: name,
            groupId: groupId
        });
    }

    inviteCharacter(groupId: number, characterId: number): Observable<CharacterInviteInfo> {
        return this.httpClient.post<CharacterInviteInfo>('/api/character/invite', {
            characterId: characterId,
            groupId: groupId,
            fromGroup: true
        });
    }

    listGroups(): Observable<PartialGroup[]> {
        return this.httpClient.get<PartialGroup[]>('/api/character/listGroups');
    }

    kickCharacter(groupId: number, characterId: number): Observable<number> {
        return this.httpClient.post<number>('/api/group/kickCharacter', {
            groupId: groupId,
            characterId: characterId,
        });
    }
}
