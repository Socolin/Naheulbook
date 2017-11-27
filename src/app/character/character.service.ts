import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable} from 'rxjs/Rx';

import {CharacterSummary, JsonService, IMetadata, ActiveStatsModifier, HistoryEntry} from '../shared';
import {NotificationsService} from '../notifications';

import {LoginService} from '../user';
import {Skill, SkillService} from '../skill';
import {Job, JobService} from '../job';
import {Origin, OriginService} from '../origin';
import {Loot} from '../loot';

import {
    Character,
    CharacterGiveDestination,
    CharacterJsonData,
} from './character.model';

@Injectable()
export class CharacterService extends JsonService {
    constructor(http: Http
        , notification: NotificationsService
        , loginService: LoginService
        , private _jobService: JobService
        , private _skillService: SkillService
        , private _originService: OriginService) {
        super(http, notification, loginService);
    }

    getCharacter(id: number): Observable<Character> {
        return Observable.forkJoin(
            this._jobService.getJobList(),
            this._originService.getOriginList(),
            this._skillService.getSkillsById(),
            this.loadLoots(id),
            this.postJson('/api/character/detail', {id: id}).map(res => res.json())
        ).map(([jobs, origins, skillsById, loots, characterData]:
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
        });
    }

    setStatBonusAD(id: number, stat: string): Observable<string> {
        return this.postJson('/api/character/setStatBonusAD', {
            id: id,
            stat: stat
        }).map(res => res.json());
    }

    LevelUp(id: number, levelUpInfo: Object): Observable<any> {
        return Observable.forkJoin(
            this._skillService.getSkills(),
            this.postJson('/api/character/levelUp', {
                id: id,
                levelUpInfo: levelUpInfo
            }).map(res => res.json()))
            .map(([skills, character]: [Skill[], Character]) => {
                for (let i = 0; i < character.skills.length; i++) {
                    let characterSkill = character.skills[i];
                    for (let j = 0; j < skills.length; j++) {
                        let skill = skills[j];
                        if (skill.id === characterSkill.id) {
                            character.skills[i] = skill;
                            break;
                        }
                    }
                }
                return character;
            }
        );
    }

    loadCharactersResume(list: number[]): Observable<CharacterSummary[]> {
        return Observable.forkJoin(
            this._jobService.getJobList(),
            this._originService.getOriginList(),
            this.postJson('/api/character/resumeList', list).map(res => res.json())
        ).map(([jobs, origins, characters]: [Job[], Origin[], CharacterSummary[]]) => {
                for (let i = 0; i < characters.length; i++) {
                    let character = characters[i];
                    character.jobs = [];
                    for (let jobId of character.jobIds) {
                        for (let job of jobs) {
                            if (job.id === jobId) {
                                character.jobs.push(job.name);
                            }
                        }
                    }

                    for (let origin of origins) {
                        if (origin.id === character.originId) {
                            character.origin = origin.name;
                            break;
                        }
                    }
                }
                return characters;
            }
        );
    }

    loadList(): Observable<CharacterSummary[]> {
        return Observable.forkJoin(
            this._jobService.getJobList(),
            this._originService.getOriginList(),
            this._http.get('/api/character/list').map(res => res.json())
        ).map(([jobs, origins, characters]: [Job[], Origin[], CharacterSummary[]]) => {
                for (let i = 0; i < characters.length; i++) {
                    let character = characters[i];
                    character.jobs = [];
                    for (let jobId of character.jobIds) {
                        for (let job of jobs) {
                            if (job.id === jobId) {
                                character.jobs.push(job.name);
                            }
                        }
                    }

                    for (let origin of origins) {
                        if (origin.id === character.originId) {
                            character.origin = origin.name;
                            break;
                        }
                    }
                }
                return characters;
            }
        );
    }

    changeCharacterStat(characterId: number, stat: string, value: any): Observable<{stat: string, value: any}> {
        return this.postJson('/api/character/update', {
            characterId: characterId,
            stat: stat,
            value: value
        }).map(res => res.json());
    }

    changeGmData(characterId: number, key: string, value: any): Observable<{key: string, value: any}> {
        return this.postJson('/api/character/updateGmData', {
            characterId: characterId,
            key: key,
            value: value
        }).map(res => res.json());
    }

    addJob(characterId: number, jobId: number): Observable<Job> {
        return Observable.forkJoin(
            this.postJson('/api/character/addJob', {
                characterId: characterId,
                jobId: jobId,
            }).map(res => res.json()),
            this._jobService.getJobsById()
        ).map(([data, jobsById]: [any, {[jobId: number]: Job}]) => {
            return jobsById[data.jobId];
        });
    }

    removeJob(characterId: number, jobId: number): Observable<Job> {
        return Observable.forkJoin(
            this.postJson('/api/character/removeJob', {
                characterId: characterId,
                jobId: jobId,
            }).map(res => res.json()),
            this._jobService.getJobsById()
        ).map(([data, jobsById]: [any, {[jobId: number]: Job}]) => {
            return jobsById[data.jobId];
        });
    }

    addModifier(characterId: number, modifier: ActiveStatsModifier): Observable<ActiveStatsModifier> {
        return this.postJson('/api/character/addModifier', {
            characterId: characterId,
            modifier: modifier,
        }).map(res => ActiveStatsModifier.fromJson(res.json()));
    }

    removeModifier(characterId: number, modifierId: number): Observable<ActiveStatsModifier> {
        return this.postJson('/api/character/removeModifier', {
            characterId: characterId,
            modifierId: modifierId,
        }).map(res => res.json());
    }

    toggleModifier(characterId: number, modifierId: number): Observable<ActiveStatsModifier> {
        return this.postJson('/api/character/toggleModifier', {
            characterId: characterId,
            modifierId: modifierId,
        }).map(res => res.json());
    }

    loadHistory(characterId, page): Observable<HistoryEntry[]> {
        return this.postJson('/api/character/history', {
            characterId: characterId,
            page: page
        }).map(res => res.json());
    }

    cancelInvite(characterId: number, groupId: number): Observable<{group: IMetadata; character: IMetadata}> {
        return this.postJson('/api/character/cancelInvite', {
            characterId: characterId,
            groupId: groupId
        }).map(res => res.json());
    }

    askJoinGroup(groupId: number, characterId: number): Observable<IMetadata> {
        return this.postJson('/api/character/invite', {
            characterId: characterId,
            groupId: groupId,
            fromGroup: false
        }).map(res => res.json());
    }

    joinGroup(characterId, groupId): Observable<{group: IMetadata; character: IMetadata}> {
        return this.postJson('/api/character/joinGroup', {
            characterId: characterId,
            groupId: groupId
        }).map(res => res.json());
    }

    createCharacter(creationData): Observable<{id: number}> {
        return this.postJson('/api/character/create', creationData).map(res => res.json());
    }

    loadLoots(characterId: number): Observable<Loot[]> {
        return Observable.forkJoin(
            this.postJson('/api/character/loadLoots', {
                characterId: characterId
            }).map(res => res.json()),
            this._skillService.getSkillsById()
        ).map(([lootsJsonData, skillsById]: [any[], {[skillId: number]: Skill}]) => {
            return Loot.lootsFromJson(lootsJsonData, skillsById)
        });
    }

    listActiveCharactersInGroup(characterId: number): Observable<CharacterGiveDestination[]> {
        return this.postJson('/api/group/listActiveCharacters', {
            characterId: characterId
        }).map(res => res.json());
    }
}
