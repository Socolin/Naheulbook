import {Injectable, forwardRef, Inject} from '@angular/core';
import {Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';

import {Stat} from "../shared";
import {Character, CharacterResume, CharacterModifier, CharacterEffect} from "./character.model";
import {Effect} from "../effect";
import {IMetadata, HistoryEntry} from "../shared";
import {CharacterInviteInfo, Group} from "../group";
import {JsonService} from '../shared/json-service';
import {Job, JobService} from '../job';
import {Origin, OriginService} from '../origin';
import {Skill, SkillService} from '../skill';
import {NotificationsService} from '../notifications';
import {LoginService} from '../user';
import {Loot} from "../loot/loot.model";

@Injectable()
export class CharacterService extends JsonService {
    constructor(http: Http
        , notification: NotificationsService
        , loginService: LoginService
        , private _jobService: JobService
        , @Inject(forwardRef(()  => SkillService)) private _skillService: SkillService
        , @Inject(forwardRef(()  => OriginService)) private _originService: OriginService) {
        super(http, notification, loginService);
    }

    private handleCharacterAsResponse(url: string, data: any): Observable<Character> {
        return Observable.forkJoin(
            this._jobService.getJobList(),
            this._originService.getOriginList(),
            this._skillService.getSkills(),
            this.postJson(url, data).map(res => res.json())
        ).map(([jobs, origins, skills, characterData]: [Job[], Origin[], Skill[], Character]) => {
                let character = new Character();
                for (let propName in characterData) {
                    if (characterData.hasOwnProperty(propName)) {
                        character[propName] = characterData[propName];
                    }
                }
                for (let j = 0; j < jobs.length; j++) {
                    let job = jobs[j];
                    if (job.id === character.jobId) {
                        character.job = job;
                        break;
                    }
                }
                for (let j = 0; j < origins.length; j++) {
                    let origin = origins[j];
                    if (origin.id === character.originId) {
                        character.origin = origin;
                        break;
                    }
                }
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
                for (let i = 0; i < character.items.length; i++) {
                    let item = character.items[i];
                    for (let k = 0; k < item.template.skills.length; k++) {
                        let itemSkill = item.template.skills[k];
                        for (let j = 0; j < skills.length; j++) {
                            let skill = skills[j];
                            if (skill.id === itemSkill.id) {
                                item.template.skills[k] = skill;
                                break;
                            }
                        }
                    }
                    for (let k = 0; k < item.template.unskills.length; k++) {
                        let itemSkill = item.template.unskills[k];
                        for (let j = 0; j < skills.length; j++) {
                            let skill = skills[j];
                            if (skill.id === itemSkill.id) {
                                item.template.unskills[k] = skill;
                                break;
                            }
                        }
                    }
                }
                try {
                    character.update();
                } catch (e) {
                    console.log(e, e.stack);
                    throw e;
                }
                return character;
            }
        );
    }

    getCharacter(id: number): Observable<Character> {
        return this.handleCharacterAsResponse('/api/character/detail', {id: id});
    }

    setStatBonusAD(id: number, stat: string): Observable<string> {
        return this.postJson('/api/character/setStatBonusAD', {
            id: id,
            stat: stat
        }).map(res => res.json());
    }

    LevelUp(id: number, levelUpInfo: Object): Observable<any> {
        return this.postJson('/api/character/levelUp', {
            id: id,
            levelUpInfo: levelUpInfo
        }).map(res => res.json());
    }

    private stats: ReplaySubject<Stat[]>;

    getStats(): Observable<Stat[]> {
        if (!this.stats) {
            this.stats = new ReplaySubject<Stat[]>(1);

            this._http.get('/api/character/stats')
                .map(res => res.json())
                .subscribe(
                    stats => {
                        this.stats.next(stats);
                    },
                    error => {
                        this.stats.error(error);
                    }
                );
        }
        return this.stats;
    }

    loadCharactersResume(list: number[]): Observable<CharacterResume[]> {
        return Observable.forkJoin(
            this._jobService.getJobList(),
            this._originService.getOriginList(),
            this.postJson('/api/character/resumeList', list).map(res => res.json())
        ).map(([jobs, origins, characters]: [Job[], Origin[], CharacterResume[]]) => {
                for (let i = 0; i < characters.length; i++) {
                    let character = characters[i];
                    for (let j = 0; j < jobs.length; j++) {
                        let job = jobs[j];
                        if (job.id === character.jobId) {
                            character.job = job.name;
                            break;
                        }
                    }
                    for (let j = 0; j < origins.length; j++) {
                        let origin = origins[j];
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
    loadList(): Observable<CharacterResume[]> {
        return Observable.forkJoin(
            this._jobService.getJobList(),
            this._originService.getOriginList(),
            this._http.get('/api/character/list').map(res => res.json())
        ).map(([jobs, origins, characters]: [Job[], Origin[], CharacterResume[]]) => {
                for (let i = 0; i < characters.length; i++) {
                    let character = characters[i];
                    for (let j = 0; j < jobs.length; j++) {
                        let job = jobs[j];
                        if (job.id === character.jobId) {
                            character.job = job.name;
                            break;
                        }
                    }
                    for (let j = 0; j < origins.length; j++) {
                        let origin = origins[j];
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

    addEffect(characterId: number, effectId: number, data: any): Observable<CharacterEffect> {
        return this.postJson('/api/character/addEffect', {
            characterId: characterId,
            effectId: effectId,
            data: data,
            active: true,
        }).map(res => res.json());
    }

    removeEffect(characterId: number, charEffect: CharacterEffect): Observable<Effect> {
        return this.postJson('/api/character/removeEffect', {
            characterId: characterId,
            charEffectId: charEffect.id,
        }).map(res => res.json());
    }

    toggleEffect(characterId: number, charEffect: CharacterEffect): Observable<Effect> {
        return this.postJson('/api/character/toggleEffect', {
            characterId: characterId,
            charEffectId: charEffect.id,
        }).map(res => res.json());
    }

    addModifier(characterId: number, modifier: CharacterModifier): Observable<CharacterModifier> {
        return this.postJson('/api/character/addModifier', {
            characterId: characterId,
            modifier: modifier,
        }).map(res => res.json());
    }

    removeModifier(characterId: number, modifierId: number): Observable<CharacterModifier> {
        return this.postJson('/api/character/removeModifier', {
            characterId: characterId,
            modifierId: modifierId,
        }).map(res => res.json());
    }

    toggleModifier(characterId: number, modifierId: number): Observable<CharacterModifier> {
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

    searchPlayersForInvite(name: string, groupId: number): Observable<CharacterInviteInfo[]> {
        return this.postJson('/api/character/searchForInvite', {
            name: name,
            groupId: groupId
        }).map(res => res.json());
    }

    cancelInvite(characterId: number, groupId: number): Observable<{group: IMetadata; character: IMetadata}> {
        return this.postJson('/api/character/cancelInvite', {
            characterId: characterId,
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

    askJoinGroup(groupId: number, characterId: number): Observable<IMetadata> {
        return this.postJson('/api/character/invite', {
            characterId: characterId,
            groupId: groupId,
            fromGroup: false
        }).map(res => res.json());
    }

    getGroup(groupId): Observable<Group> {
        return this.postJson('/api/character/groupDetail', {
            groupId: groupId
        }).map(res => res.json());
    }

    createGroup(name): Observable<Group> {
        return this.postJson('/api/character/createGroup', {
            name: name
        }).map(res => res.json());
    }

    joinGroup(characterId, groupId): Observable<{group: IMetadata; character: IMetadata}> {
        return this.postJson('/api/character/joinGroup', {
            characterId: characterId,
            groupId: groupId
        }).map(res => res.json());
    }

    listGroups(): Observable<Object[]> {
        return this._http.get('/api/character/listGroups').map(res => res.json());
    }

    createCharacter(creationData): Observable<{id: number}> {
        return this.postJson('/api/character/create', creationData).map(res => res.json());
    }

    loadLoots(characterId: number): Observable<Loot[]> {
        return this.postJson('/api/character/loadLoots', {
            characterId: characterId
        }).map(res => res.json());
    }
}
