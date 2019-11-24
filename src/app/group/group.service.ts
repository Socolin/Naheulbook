
import {forkJoin, of as observableOf, Observable, Subject, of} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {FighterDurationChanges, HistoryEntry} from '../shared';
import {Monster} from '../monster';
import {Loot} from '../loot';
import {NEvent, EventService} from '../event';
import {Character, CharacterService} from '../character';
import {NhbkDate, NhbkDateOffset} from '../date';
import {Skill, SkillService} from '../skill';

import {Group, GroupInviteResponse, Npc} from './group.model';
import {
    CharacterSearchResponse,
    GroupResponse,
    GroupSummaryResponse,
    LootResponse,
    NpcResponse
} from '../api/responses';
import {NpcRequest} from '../api/requests';

@Injectable()
export class GroupService {
    constructor(
        private readonly httpClient: HttpClient,
        private readonly characterService: CharacterService,
        private readonly eventService: EventService,
        private readonly skillService: SkillService,
    ) {
    }

    getGroup(groupId): Observable<Group> {
        let subject = new Subject<Group>();

        this.httpClient.get<GroupResponse>(`/api/v2/groups/${groupId}`).subscribe((groupData: GroupResponse) => {
            let charactersLoading: Observable<Character|null>[] = [];
            for (let i = 0; i < groupData.characterIds.length; i++) {
                let characterId = groupData.characterIds[i];
                charactersLoading.push(this.characterService.getCharacter(characterId));
            }

            let group = Group.fromJson(groupData);

            if (charactersLoading.length === 0) {
                charactersLoading.push(observableOf(null));
            }
            forkJoin([
                forkJoin(charactersLoading),
                this.loadMonsters(group.id),
                this.loadLoots(group.id),
                this.eventService.loadEvents(group.id)
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
                group.updateFightersOrder(true);
                subject.next(group);
                subject.complete();
            });
        });
        return subject;
    }

    createGroup(name): Observable<GroupResponse> {
        return this.httpClient.post<GroupResponse>('/api/v2/groups', {
            name: name
        });
    }

    /* Monster */

    loadMonsters(groupId: number): Observable<Monster[]> {
        return forkJoin([
            this.httpClient.get<Monster[]>(`/api/v2/groups/${groupId}/monsters`),
            this.skillService.getSkillsById()
        ]).pipe(map(([monstersJsonData, skillsById]: [any[], {[skillId: number]: Skill}]) => {
            return Monster.monstersFromJson(monstersJsonData, skillsById)
        }));
    }

    loadDeadMonsters(groupId: number, startIndex: number, count: number): Observable<Monster[]> {
        return forkJoin([
            this.httpClient.get<Monster[]>(`/api/v2/groups/${groupId}/deadMonsters?startIndex=${startIndex}&count=${count}`),
            this.skillService.getSkillsById()
        ]).pipe(map(([monstersJsonData, skillsById]: [any[], {[skillId: number]: Skill}]) => {
            return Monster.monstersFromJson(monstersJsonData, skillsById)
        }));
    }

    /* Loot */

    loadLoots(groupId: number): Observable<Loot[]> {
        return forkJoin([
            this.httpClient.get<LootResponse[]>(`/api/v2/groups/${groupId}/loots`),
            this.skillService.getSkillsById()
        ]).pipe(map(([lootsJsonData, skillsById]) => {
            return Loot.lootsFromJson(lootsJsonData, skillsById)
        }));
    }

    createLoot(groupId: number, lootName: string): Observable<Loot> {
        return forkJoin([
            this.httpClient.post<LootResponse>(`/api/v2/groups/${groupId}/loots`, {
                name: lootName
            }),
            this.skillService.getSkillsById()
        ]).pipe(map(([lootJsonData, skillsById]) => {
            return Loot.fromResponse(lootJsonData, skillsById)
        }));
    }

    deleteLoot(lootId: number): Observable<void> {
        return this.httpClient.delete<void>(`/api/v2/loots/${lootId}`);
    }

    updateLootVisibility(lootId: number, visibleForPlayer: boolean): Observable<void> {
        return this.httpClient.put<void>(`/api/v2/loots/${lootId}/visibility`, {
            visibleForPlayer: visibleForPlayer,
        });
    }

    /* History */

    loadHistory(groupId: number, page: number): Observable<HistoryEntry[]> {
        return this.httpClient.get<HistoryEntry[]>(`/api/v2/groups/${groupId}/history?page=${page}`);
    }

    addLog(groupId: number, info: string, isGm: boolean): Observable<Object> {
        return this.httpClient.post<Object>(`/api/v2/groups/${groupId}/history`, {
            isGm: isGm,
            info: info
        });
    }

    /* Misc */

    editGroupLocation(groupId: number, locationId: number): Observable<void> {
        return this.httpClient.put<void>(`/api/v2/groups/${groupId}/location`, {
            locationId
        });
    }

    editGroupValue(groupId: number, key: string, value: any): Observable<void> {
        return this.httpClient.patch<void>(`/api/v2/groups/${groupId}/`, {
            [key]: value
        });
    }

    addTime(groupId: number, dateOffset: NhbkDateOffset): Observable<NhbkDate> {
        return this.httpClient.post<NhbkDate>(`/api/v2/groups/${groupId}/addTime`, dateOffset);
    }

    saveChangedTime(groupId: number, changes: FighterDurationChanges[]): Observable<void> {
        if (changes.length === 0) {
            return of(void 0);
        }
        return this.httpClient.post<void>(`/api/v2/groups/${groupId}/updateDurations`, changes);
    }

    searchPlayersForInvite(name: string): Observable<CharacterSearchResponse[]> {
        return this.httpClient.get<CharacterSearchResponse[]>('/api/v2/characters/search?filter=' + encodeURIComponent(name));
    }

    inviteCharacter(groupId: number, characterId: number): Observable<GroupInviteResponse> {
        return this.httpClient.post<GroupInviteResponse>(`/api/v2/groups/${groupId}/invites`, {
            characterId: characterId,
            fromGroup: true
        });
    }

    listGroups(): Observable<GroupSummaryResponse[]> {
        return this.httpClient.get<GroupSummaryResponse[]>('/api/v2/groups');
    }

    kickCharacter(groupId: number, characterId: number): Observable<number> {
        return this.httpClient.post<number>('/api/group/kickCharacter', {
            groupId: groupId,
            characterId: characterId,
        });
    }

    startCombat(groupId: number): Observable<void> {
        return this.httpClient.post<void>(`/api/v2/groups/${groupId}/startCombat`, {});
    }

    endCombat(groupId: number): Observable<void> {
        return this.httpClient.post<void>(`/api/v2/groups/${groupId}/endCombat`, {});
    }

    createNpc(groupId: number, request: NpcRequest): Observable<Npc> {
        return this.httpClient.post<NpcResponse>(`/api/v2/groups/${groupId}/npcs`, request)
            .pipe(map((response) => Npc.fromResponse(response)));
    }

    getNpcs(groupId: number): Observable<Npc[]> {
        return this.httpClient.get<NpcResponse[]>(`/api/v2/groups/${groupId}/npcs`)
            .pipe(map(responses => Npc.fromResponses(responses)));
    }

    editNpc(npcId: number, request: NpcRequest): Observable<Npc> {
        return this.httpClient.put<NpcResponse>(`/api/v2/npcs/${npcId}`, request)
            .pipe(map(response => Npc.fromResponse(response)));
    }
}
