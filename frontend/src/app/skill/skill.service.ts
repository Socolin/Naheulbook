import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, ReplaySubject} from 'rxjs';

import {Skill, SkillDictionary} from './skill.model';
import {SkillResponse} from '../api/responses';

@Injectable({providedIn: 'root'})
export class SkillService {
    private skills: ReplaySubject<Skill[]>;
    private skillsById: ReplaySubject<SkillDictionary>;

    constructor(private httpClient: HttpClient) {
    }

    getSkillsById(): Observable<SkillDictionary> {
        if (!this.skillsById) {
            this.skillsById = new ReplaySubject<SkillDictionary>(1);

            this.getSkills().subscribe(
                skills => {
                    let skillsById: SkillDictionary = {};
                    for (let skill of skills) {
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

            this.httpClient.get<SkillResponse[]>('/api/v2/skills')
                .subscribe(
                    skillResponses => {
                        let skills: Skill[] = [];
                        for (let response of skillResponses) {
                            skills.push(Skill.fromResponse(response));
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
