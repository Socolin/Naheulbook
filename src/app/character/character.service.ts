import {forkJoin, Observable} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {ActiveStatsModifier, HistoryEntry, IMetadata} from '../shared';

import {Skill, SkillService} from '../skill';
import {Job, JobService} from '../job';
import {OriginService} from '../origin';
import {Loot} from '../loot';

import {Character, CharacterGiveDestination} from './character.model';
import {LevelUpInfo} from './character.component';
import {IActiveStatsModifier} from '../api/shared';
import {CreateCharacterRequest} from '../api/requests/create-character-request';
import {CharacterLevelUpRequest} from '../api/requests';
import {
    CharacterFoGmResponse,
    CharacterLevelUpResponse,
    CharacterResponse,
    CharacterSummaryResponse,
    DeleteInviteResponse,
    ListActiveCharacterResponse,
    RandomCharacterNameResponse
} from '../api/responses';

@Injectable()
export class CharacterService {
    constructor(
        private readonly httpClient: HttpClient,
        private readonly jobService: JobService,
        private readonly skillService: SkillService,
        private readonly originService: OriginService,
    ) {
    }

    getCharacter(id: number): Observable<Character> {
        return forkJoin([
            this.jobService.getJobList(),
            this.originService.getOriginList(),
            this.skillService.getSkillsById(),
            this.loadLoots(id),
            this.httpClient.get<CharacterResponse | CharacterFoGmResponse>(`/api/v2/characters/${id}`)
        ]).pipe(map(([jobs, origins, skillsById, loots, characterData]) => {
            let character = Character.fromResponse(characterData, origins, jobs, skillsById);

            for (let i = 0; i < loots.length; i++) {
                let loot = loots[i];
                character.addLoot(loot);
            }

            try {
                character.update();
            } catch (e) {
                console.log(e, e.stack);
                throw e;
            }
            return character;
        }));
    }

    setStatBonusAD(id: number, stat: string): Observable<string> {
        return this.httpClient.put(`/api/v2/characters/${id}/statBonusAd`, {
            stat: stat
        }).pipe(map(() => stat));
    }

    LevelUp(characterId: number, levelUpInfo: LevelUpInfo): Observable<[CharacterLevelUpResponse, { [skillId: number]: Skill }]> {
        let skillId: number | undefined = undefined;
        if (levelUpInfo.skill) {
            skillId = levelUpInfo.skill.id;
        }
        let specialityIds: number[] = [];
        for (let jobId in levelUpInfo.specialities) {
            if (!levelUpInfo.specialities.hasOwnProperty(jobId)) {
                continue;
            }
            const speciality = levelUpInfo.specialities[jobId];
            if (!speciality) {
                continue;
            }
            specialityIds.push(speciality.id);
        }

        let request: CharacterLevelUpRequest = {
            evOrEa: levelUpInfo.evOrEa,
            evOrEaValue: levelUpInfo.evOrEaValue,
            targetLevelUp: levelUpInfo.targetLevelUp,
            statToUp: levelUpInfo.statToUp,
            skillId: skillId,
            specialityIds: specialityIds
        };

        return forkJoin([
            this.httpClient.post<CharacterLevelUpResponse>(`/api/v2/characters/${characterId}/levelUp`, request),
            this.skillService.getSkillsById()
        ]);
    }

    loadList(): Observable<CharacterSummaryResponse[]> {
        return this.httpClient.get<CharacterSummaryResponse[]>('/api/v2/characters');
    }

    changeCharacterStat(characterId: number, stat: string, value: any): Observable<{ stat: string, value: any }> {
        return this.httpClient.patch<{ stat: string, value: any }>(`/api/v2/characters/${characterId}/`, {
            [stat]: value
        }).pipe(map(() => {
            return {stat, value}
        }));
    }

    changeGmData(characterId: number, key: string, value: any): Observable<{ key: string, value: any }> {
        return this.httpClient.patch<{ stat: string, value: any }>(`/api/v2/characters/${characterId}/`, {
            [key]: value
        }).pipe(map(() => {
            return {key, value}
        }));
    }

    addJob(characterId: number, jobId: number): Observable<Job> {
        return forkJoin([
            this.httpClient.post('/api/character/addJob', {
                characterId: characterId,
                jobId: jobId,
            }),
            this.jobService.getJobsById()
        ]).pipe(map(([data, jobsById]: [any, { [jobId: number]: Job }]) => {
            return jobsById[data.jobId];
        }));
    }

    removeJob(characterId: number, jobId: number): Observable<Job> {
        return forkJoin([
            this.httpClient.post('/api/character/removeJob', {
                characterId: characterId,
                jobId: jobId,
            }),
            this.jobService.getJobsById()
        ]).pipe(map(([data, jobsById]: [any, { [jobId: number]: Job }]) => {
            return jobsById[data.jobId];
        }));
    }

    addModifier(characterId: number, modifier: ActiveStatsModifier): Observable<ActiveStatsModifier> {
        return this.httpClient.post<IActiveStatsModifier>(`/api/v2/characters/${characterId}/modifiers`, modifier)
            .pipe(map(ActiveStatsModifier.fromJson));
    }

    removeModifier(characterId: number, modifierId: number): Observable<number> {
        return this.httpClient.delete(`/api/v2/characters/${characterId}/modifiers/${modifierId}`)
            .pipe(map(() => modifierId));
    }

    toggleModifier(characterId: number, modifierId: number): Observable<ActiveStatsModifier> {
        return this.httpClient.post<ActiveStatsModifier>(`/api/v2/characters/${characterId}/modifiers/${modifierId}/toggle`, {});
    }

    loadHistory(characterId, page): Observable<HistoryEntry[]> {
        return this.httpClient.get<HistoryEntry[]>(`/api/v2/characters/${characterId}/history?page=${page}`);
    }

    cancelInvite(characterId: number, groupId: number): Observable<DeleteInviteResponse> {
        return this.httpClient.delete<DeleteInviteResponse>(`/api/v2/groups/${groupId}/invites/${characterId}`);
    }

    askJoinGroup(groupId: number, characterId: number): Observable<void> {
        return this.httpClient.post<void>(`/api/v2/groups/${groupId}/invites`, {
            characterId: characterId,
            fromGroup: false
        });
    }

    joinGroup(characterId: number, groupId: number): Observable<void> {
        return this.httpClient.post<void>(`/api/v2/groups/${groupId}/invites/${characterId}/accept`, {});
    }

    createCharacter(creationData: CreateCharacterRequest): Observable<{ id: number }> {
        return this.httpClient.post<{ id: number }>('/api/v2/characters', creationData);
    }

    loadLoots(characterId: number): Observable<Loot[]> {
        return forkJoin([
            this.httpClient.get(`/api/v2/characters/${characterId}/loots`),
            this.skillService.getSkillsById()
        ]).pipe(map(([lootsJsonData, skillsById]: [any[], { [skillId: number]: Skill }]) => {
            return Loot.lootsFromJson(lootsJsonData, skillsById)
        }));
    }

    listActiveCharactersInGroup(groupId: number): Observable<CharacterGiveDestination[]> {
        return this.httpClient.get<ListActiveCharacterResponse[]>(`/api/v2/groups/${groupId}/activeCharacters`);
    }

    createCustomCharacter(customCharacterData: any): Observable<IMetadata> {
        return this.httpClient.post<IMetadata>('/api/character/createCustom', customCharacterData);
    }

    getRandomName(originId: number, sex: string) {
        return this.httpClient.get<RandomCharacterNameResponse>(`/api/v2/origins/${originId}/randomCharacterName`, {
            params: {
                sex: sex
            }
        }).pipe(map((s) => s.name));
    }
}
