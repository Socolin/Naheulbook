import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ReplaySubject, Observable} from 'rxjs';

import {Skill, SkillDictionary} from './skill.model';

@Injectable()
export class SkillService {
    private skills: ReplaySubject<Skill[]>;
    private skillsById: ReplaySubject<SkillDictionary>;

    constructor(private httpClient: HttpClient) {
    }

    getSkillsById(): Observable<SkillDictionary> {
        if (!this.skillsById) {
            this.skillsById = new ReplaySubject<SkillDictionary>(1);

            this.getSkills().subscribe(
                skillsJsonData => {
                    let skillsById: SkillDictionary = {};
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

            this.httpClient.get<any[]>('/api/v2/skills')
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
