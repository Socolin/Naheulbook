import {Injectable, EventEmitter} from '@angular/core';
import {Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';

import {Skill} from "./skill.model";

@Injectable()
export class SkillService {
    private skills: ReplaySubject<Skill[]>;

    constructor(private _http: Http) {
    }

    getSkill(skillId: number): Observable<Skill> {
        let skill: EventEmitter<Skill> = new EventEmitter<Skill>();
        this.getSkills().subscribe(skills => {
            for (let i = 0; i < skills.length; i++) {
                if (skills[i].id === skillId) {
                    skill.emit(skills[i]);
                    return;
                }
            }
            skill.emit(null);
        });
        return skill;
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
