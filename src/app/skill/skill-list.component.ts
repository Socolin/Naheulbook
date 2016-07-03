import {Component} from '@angular/core';

import {SkillComponent} from './skill.component';
import {Skill} from "./skill.model";
import {SkillService} from "./skill.service";

import {removeDiacritics} from "../shared";

@Component({
    selector: 'skill-list',
    templateUrl: 'app/skill/skill-list.component.html',
    styleUrls: ['/styles/skill-list.css'],
    directives: [SkillComponent]
})
export class SkillListComponent {
    public skills: Skill[];
    public filter: string;

    constructor(private _skillService: SkillService) {
    }

    isFiltered(skill: Skill): boolean {
        if (!this.filter) {
            return true;
        }
        let cleanFilter = removeDiacritics(this.filter).toLowerCase();
        return removeDiacritics(skill.name).toLowerCase().indexOf(cleanFilter) > -1;
    }

    getSkills() {
        this._skillService.getSkills().subscribe(
            skills => {
                this.skills = skills;
            });
    }

    ngOnInit() {
        this.getSkills();
    }
}

