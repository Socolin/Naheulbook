import {Injectable} from '@angular/core';
import {Response, Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';

import {Stat} from "../shared/stat.model";
import {Character, CharacterResume, CharacterModifier} from "./character.model";
import {Effect} from "../effect";
import {IMetadata, HistoryEntry} from "../shared";
import {CharacterInviteInfo, Group} from "../group";


@Injectable()
export class CharacterService {
    constructor(private _http: Http) {
    }

    getCharacter(id: number): Observable<Character> {
        return this._http.post('/api/character/detail', JSON.stringify({
            id: id
        })).map(res => res.json());
    }

    setStatBonusAD(id: number, stat: string): Observable<Response> {
        return this._http.post('/api/character/setStatBonusAD', JSON.stringify({
            id: id,
            stat: stat
        }));
    }

    LevelUp(id: number, levelUpInfo: Object): Observable<Response> {
        return this._http.post('/api/character/levelUp', JSON.stringify({
            id: id,
            levelUpInfo: levelUpInfo
        }));
    }


    private stats: ReplaySubject<Stat[]>;

    getStats(): Observable<Stat[]> {
        if (!this.stats || this.stats.isUnsubscribed) {
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

    loadList(): Observable<CharacterResume[]> {
        return this._http.get('/api/character/list')
            .map(res => res.json());
    }

    changeCharacterStat(characterId: number, stat: string, value: any): Observable<{stat: string, value: any}> {
        return this._http.post('/api/character/update', JSON.stringify({
            characterId: characterId,
            stat: stat,
            value: value
        })).map(res => res.json());
    }

    changeGmData(characterId: number, key: string, value: any): Observable<{key: string, value: any}> {
        return this._http.post('/api/character/updateGmData', JSON.stringify({
            characterId: characterId,
            key: key,
            value: value
        })).map(res => res.json());
    }

    addEffect(characterId: number, effectId: number): Observable<Effect> {
        return this._http.post('/api/character/addEffect', JSON.stringify({
            characterId: characterId,
            effectId: effectId,
        })).map(res => res.json());
    }

    removeEffect(characterId: number, effectId: number): Observable<Effect> {
        return this._http.post('/api/character/removeEffect', JSON.stringify({
            characterId: characterId,
            effectId: effectId,
        })).map(res => res.json());
    }

    addModifier(characterId: number, modifier: CharacterModifier): Observable<CharacterModifier> {
        return this._http.post('/api/character/addModifier', JSON.stringify({
            characterId: characterId,
            modifier: modifier,
        })).map(res => res.json());
    }

    removeModifier(characterId: number, modifierId: number): Observable<CharacterModifier> {
        return this._http.post('/api/character/removeModifier', JSON.stringify({
            characterId: characterId,
            modifierId: modifierId,
        })).map(res => res.json());
    }

    loadHistory(characterId, page): Observable<HistoryEntry[]> {
        return this._http.post('/api/character/history', JSON.stringify({
            characterId: characterId,
            page: page
        })).map(res => res.json());
    }

    searchPlayersForInvite(name: string, groupId: number): Observable<CharacterInviteInfo[]> {
        return this._http.post('/api/character/searchForInvite', JSON.stringify({
            name: name,
            groupId: groupId
        })).map(res => res.json());
    }

    cancelInvite(characterId: number, groupId: number): Observable<{group: IMetadata; character: IMetadata}> {
        return this._http.post('/api/character/cancelInvite', JSON.stringify({
            characterId: characterId,
            groupId: groupId
        })).map(res => res.json());
    }

    inviteCharacter(groupId: number, characterId: number): Observable<CharacterInviteInfo> {
        return this._http.post('/api/character/invite', JSON.stringify({
            characterId: characterId,
            groupId: groupId,
            fromGroup: 1
        })).map(res => res.json());
    }

    askJoinGroup(groupId: number, characterId: number): Observable<CharacterInviteInfo> {
        return this._http.post('/api/character/invite', JSON.stringify({
            characterId: characterId,
            groupId: groupId,
            fromGroup: 0
        })).map(res => res.json());
    }

    getGroup(groupId): Observable<Group> {
        return this._http.post('/api/character/groupDetail', JSON.stringify({
            groupId: groupId
        })).map(res => res.json());
    }

    createGroup(name): Observable<Group> {
        return this._http.post('/api/character/createGroup', JSON.stringify({
            name: name
        })).map(res => res.json());
    }

    joinGroup(characterId, groupId): Observable<{group: IMetadata; character: IMetadata}> {
        return this._http.post('/api/character/joinGroup', JSON.stringify({
            characterId: characterId,
            groupId: groupId
        })).map(res => res.json());
    }

    listGroups(): Observable<Object[]> {
        return this._http.get('/api/character/listGroups').map(res => res.json());
    }

    saveStatsCache(characterId, stats): Observable<Object> {
        return this._http.post('/api/character/saveStatsCache', JSON.stringify({
            id: characterId,
            stats: stats
        })).map(res => res.json());
    }

    createCharacter(creationData): Observable<{id: number}> {
        return this._http.post('/api/character/create', JSON.stringify(creationData)).map(res => res.json());
    }
}
