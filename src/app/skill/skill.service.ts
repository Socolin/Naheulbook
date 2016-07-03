import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';

import {Skill} from "./skill.model";

@Injectable()
export class SkillService {
    private skills: ReplaySubject<Skill[]>;

    constructor(private _http: Http) {
    }

    getSkills(): Observable<Skill[]> {
        if (!this.skills || this.skills.isUnsubscribed) {
            this.skills = new ReplaySubject<Skill[]>(1);

            this._http.get('/api/skill/list')
                .map(res => res.json())
                .subscribe(
                    skills => {
                        this.skills.next(skills);
                    },
                    error => {
                        this.skills.error(error);
                    }
                );
        }
        return this.skills;
    }
}
