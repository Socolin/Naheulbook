import {Component, OnInit} from '@angular/core';

import {SkillComponent} from './skill.component';
import {Skill} from "./skill.model";
import {SkillService} from "./skill.service";

import {removeDiacritics} from "../shared";

@Component({
    moduleId: module.id,
    selector: 'skill-list',
    templateUrl: 'skill-list.component.html',
    directives: [SkillComponent]
})
export class SkillListComponent implements OnInit {
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

