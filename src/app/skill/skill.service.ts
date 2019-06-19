import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';

import {JsonService} from '../shared';
import {NotificationsService} from '../notifications';
import {LoginService} from '../user';

import {Skill} from './skill.model';

@Injectable()
export class SkillService extends JsonService {
    private skills: ReplaySubject<Skill[]>;
    private skillsById: ReplaySubject<{[skillId: number]: Skill}>;

    constructor(http: Http, notifications: NotificationsService, login: LoginService) {
        super(http, notifications, login);
    }

    getSkillsById(): Observable<{[skillId: number]: Skill}> {
        if (!this.skillsById) {
            this.skillsById = new ReplaySubject<{[skillId: number]: Skill}>(1);

            this.getSkills().subscribe(
                skillsJsonData => {
                    let skillsById: {[skillId: number]: Skill} = {};
                    for (let skillJsonData of skillsJsonData) {
                        skillsById[skillJsonData.id] = Skill.fromJson(skillJsonData);
                    }
                    this.skillsById.next(skillsById);
                    this.skillsById.complete();
                },
                error => {
                    this.skillsById.error(error);
                }
            );
        }
        return this.skillsById;
    }

    getSkills(): Observable<Skill[]> {
        if (!this.skills) {
            this.skills = new ReplaySubject<Skill[]>(1);

            this._http.get('/api/v2/skills')
                .map(res => res.json())
                .subscribe(
                    skillsJsonData => {
                        let skills: Skill[] = [];
                        for (let skillJsonData of skillsJsonData) {
                            skills.push(Skill.fromJson(skillJsonData));
                        }
                        this.skills.next(skills);
                        this.skills.complete();
                    },
                    error => {
                        this.skills.error(error);
                    }
                );
        }
        return this.skills;
    }
}
