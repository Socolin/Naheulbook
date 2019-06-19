
import {forkJoin, Observable, ReplaySubject} from 'rxjs';

import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

import {Skill, SkillService} from '../skill';
import {Origin} from './origin.model';

@Injectable()
export class OriginService {
    private origins: ReplaySubject<Origin[]>;

    constructor(private httpClient: HttpClient
        , private _skillService: SkillService) {
    }

    getOriginList(): Observable<Origin[]> {
        if (!this.origins) {
            this.origins = new ReplaySubject<Origin[]>(1);

            forkJoin([
                this._skillService.getSkillsById(),
                this.httpClient.get<any[]>('/api/v2/origins')
            ]).subscribe(
                ([skillsById, originsDatas]: [{[skillId: number]: Skill}, Origin[]]) => {
                    let origins: Origin[] = [];
                    for (let originData of originsDatas) {
                        let origin = Origin.fromJson(originData, skillsById);
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

    getOriginsById(): Observable<{[originId: number]: Origin}> {
        return this.getOriginList().pipe(map((origins: Origin[]) => {
            let originsById = {};
            origins.map(o => originsById[o.id] = o);
            return originsById;
        }));
    }

    getOriginsNamesById(): Observable<{[originId: number]: string}> {
        return this.getOriginList().pipe(map((origins: Origin[]) => {
            let originNamesById = {};
            origins.map(o => originNamesById[o.id] = o.name);
            return originNamesById;
        }));
    }
}
