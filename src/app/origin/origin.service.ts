import {Injectable, forwardRef, Inject} from '@angular/core';
import {Http} from '@angular/http';
import {Observable, ReplaySubject} from 'rxjs/Rx';

import {Origin} from './origin.model';
import {Skill, SkillService} from '../skill';

@Injectable()
export class OriginService {
    private origins: ReplaySubject<Origin[]>;

    constructor(private _http: Http
        , private _skillService: SkillService) {
    }

    getOriginList(): Observable<Origin[]> {
        if (!this.origins) {
            this.origins = new ReplaySubject<Origin[]>(1);

            Observable.forkJoin(
                this._skillService.getSkillsById(),
                this._http.get('/api/origin/list').map(res => res.json())
            ).subscribe(
                ([skillsById, origins]: [{[skillId: number]: Skill}, Origin[]]) => {
                    for (let i = 0; i < origins.length; i++) {
                        let origin = origins[i];
                        for (let s = 0; s < origin.skills.length; s++) {
                            let skill = origin.skills[s];
                            origin.skills[s] = skillsById[skill.id];
                        }
                        for (let s = 0; s < origin.availableSkills.length; s++) {
                            let skill = origin.availableSkills[s];
                            origin.availableSkills[s] = skillsById[skill.id];
                        }
                    }
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
}
