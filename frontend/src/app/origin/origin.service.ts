import {forkJoin, Observable, ReplaySubject} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {SkillDictionary, SkillService} from '../skill';
import {Origin, OriginDictionary} from './origin.model';
import {OriginResponse, RandomCharacterNameResponse} from '../api/responses';
import {NamesByNumericId} from '../shared/shared,model';
import {Guid} from '../api/shared/util';
import {CharacterSex} from '../api/shared/enums';

@Injectable({providedIn: 'root'})
export class OriginService {
    private origins: ReplaySubject<Origin[]>;

    constructor(
        private readonly httpClient: HttpClient,
        private readonly skillService: SkillService,
    ) {
    }

    getOriginList(): Observable<Origin[]> {
        if (!this.origins) {
            this.origins = new ReplaySubject<Origin[]>(1);

            forkJoin([
                this.skillService.getSkillsById(),
                this.httpClient.get<OriginResponse[]>('/api/v2/origins')
            ]).subscribe(
                ([skillsById, originsDatas]: [SkillDictionary, OriginResponse[]]) => {
                    let origins: Origin[] = [];
                    for (let originData of originsDatas) {
                        let origin = Origin.fromResponse(originData, skillsById);
                        Object.freeze(origin);
                        origins.push(origin);
                    }
                    Object.freeze(origins);
                    this.origins.next(origins);
                    this.origins.complete();
                },
                error => {
                    this.origins.error(error);
                }
            );
        }
        return this.origins;
    }

    getOriginsById(): Observable<OriginDictionary> {
        return this.getOriginList().pipe(map((origins: Origin[]) => {
            let originsById = {};
            origins.map(o => originsById[o.id] = o);
            return originsById;
        }));
    }

    getOriginsNamesById(): Observable<NamesByNumericId> {
        return this.getOriginList().pipe(map((origins: Origin[]) => {
            let originNamesById = {};
            origins.map(o => originNamesById[o.id] = o.name);
            return originNamesById;
        }));
    }

    getRandomName(originId: Guid, sex: CharacterSex): Observable<string> {
        return this.httpClient.get<RandomCharacterNameResponse>(`/api/v2/origins/${originId}/randomCharacterName`, {
            params: {
                sex: sex
            }
        }).pipe(
            map((s) => s.name)
        );
    }
}
