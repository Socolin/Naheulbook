import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {ReplaySubject, Observable} from 'rxjs/Rx';

import {Skill} from './skill.model';
import {JsonService} from '../shared/json-service';
import {NotificationsService} from '../notifications/notifications.service';
import {LoginService} from '../user/login.service';

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
                skills => {
                    let skillsById: {[skillId: number]: Skill} = {};
                    for (let i = 0; i < skills.length; i++) {
                        let skill = skills[i];
                        skillsById[skill.id] = skill;
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

            this.postJson('/api/skill/list')
                .map(res => res.json())
                .subscribe(
                    skills => {
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
