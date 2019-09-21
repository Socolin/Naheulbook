
import {forkJoin, Observable} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {CharacterSummary, IMetadata, ActiveStatsModifier, HistoryEntry} from '../shared';

import {Skill, SkillService} from '../skill';
import {Job, JobService} from '../job';
import {Origin, OriginService} from '../origin';
import {Loot} from '../loot';

import {
    Character,
    CharacterGiveDestination,
    CharacterJsonData, CharacterLevelUpResponse, LevelUpRequest, RandomNameResponse,
} from './character.model';
import {DeleteGroupResponse} from '../group';
import {LevelUpInfo} from './character.component';
import {IActiveStatsModifier} from '../api/shared';

@Injectable()
export class CharacterService {
    constructor(private httpClient: HttpClient
        , private _jobService: JobService
        , private _skillService: SkillService
        , private _originService: OriginService) {
    }

    getCharacter(id: number): Observable<Character> {
        return forkJoin([
            this._jobService.getJobList(),
            this._originService.getOriginList(),
            this._skillService.getSkillsById(),
            this.loadLoots(id),
            this.httpClient.get(`/api/v2/characters/${id}`)
        ]).pipe(map(([jobs, origins, skillsById, loots, characterData]:
                                            [Job[], Origin[], {[skillId: number]: Skill}, Loot[], CharacterJsonData]) => {
            let character = Character.fromJson(characterData, origins, jobs, skillsById);

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

    LevelUp(characterId: number, levelUpInfo: LevelUpInfo): Observable<[CharacterLevelUpResponse, {[skillId: number]: Skill}]> {
        let skillId = undefined;
        if (levelUpInfo.skill) {
            skillId = levelUpInfo.skill.id;
        }
        let specialityIds = [];
        for (let jobId in levelUpInfo.specialities) {
            if (!levelUpInfo.specialities.hasOwnProperty(jobId)) {
                continue;
            }
            specialityIds.push(levelUpInfo.specialities[jobId].id);
        }

        let request: LevelUpRequest = {
            evOrEa: levelUpInfo.evOrEa,
            evOrEaValue: levelUpInfo.evOrEaValue,
            targetLevelUp: levelUpInfo.targetLevelUp,
            statToUp: levelUpInfo.statToUp,
            skillId: skillId,
            specialityIds: specialityIds
        };

        return forkJoin([
            this.httpClient.post<CharacterLevelUpResponse>(`/api/v2/characters/${characterId}/levelUp`, request),
            this._skillService.getSkillsById()
        ]);
    }

    loadList(): Observable<CharacterSummary[]> {
        return this.httpClient.get<CharacterSummary[]>('/api/v2/characters');
    }

    changeCharacterStat(characterId: number, stat: string, value: any): Observable<{stat: string, value: any}> {
        return this.httpClient.patch<{stat: string, value: any}>(`/api/v2/characters/${characterId}/`, {
            [stat]: value
        }).pipe(map(() => {return {stat, value}}));
    }

    changeGmData(characterId: number, key: string, value: any): Observable<{key: string, value: any}> {
        return this.httpClient.patch<{stat: string, value: any}>(`/api/v2/characters/${characterId}/`, {
            [key]: value
        }).pipe(map(() => {return {key, value}}));
    }

    addJob(characterId: number, jobId: number): Observable<Job> {
        return forkJoin([
            this.httpClient.post('/api/character/addJob', {
                characterId: characterId,
                jobId: jobId,
            }),
            this._jobService.getJobsById()
        ]).pipe(map(([data, jobsById]: [any, {[jobId: number]: Job}]) => {
            return jobsById[data.jobId];
        }));
    }

    removeJob(characterId: number, jobId: number): Observable<Job> {
        return forkJoin([
            this.httpClient.post('/api/character/removeJob', {
                characterId: characterId,
                jobId: jobId,
            }),
            this._jobService.getJobsById()
        ]).pipe(map(([data, jobsById]: [any, {[jobId: number]: Job}]) => {
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

    cancelInvite(characterId: number, groupId: number): Observable<DeleteGroupResponse> {
        return this.httpClient.delete<DeleteGroupResponse>(`/api/v2/groups/${groupId}/invites/${characterId}`);
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

    createCharacter(creationData): Observable<{id: number}> {
        return this.httpClient.post<{id: number}>('/api/v2/characters', creationData);
    }

    loadLoots(characterId: number): Observable<Loot[]> {
        return forkJoin([
            this.httpClient.get(`/api/v2/characters/${characterId}/loots`),
            this._skillService.getSkillsById()
        ]).pipe(map(([lootsJsonData, skillsById]: [any[], {[skillId: number]: Skill}]) => {
            return Loot.lootsFromJson(lootsJsonData, skillsById)
        }));
    }

    listActiveCharactersInGroup(groupId: number): Observable<CharacterGiveDestination[]> {
        return this.httpClient.get<CharacterGiveDestination[]>(`/api/v2/groups/${groupId}/activeCharacters`);
    }

    createCustomCharacter(customCharacterData: any): Observable<IMetadata> {
        return this.httpClient.post<IMetadata>('/api/character/createCustom', customCharacterData);
    }

    getRandomName(originId: number, sex: string) {
        return this.httpClient.get<RandomNameResponse>(`/api/v2/origins/${originId}/randomCharacterName`, {
            params: {
                sex: sex
            }
        }).pipe(map((s) => s.name));
    }
}
