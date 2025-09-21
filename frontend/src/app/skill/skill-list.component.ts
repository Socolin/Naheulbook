import {Component, OnInit} from '@angular/core';

import {Skill} from './skill.model';
import {SkillService} from './skill.service';

import {removeDiacritics} from '../shared';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatCard } from '@angular/material/card';
import { SkillComponent } from './skill.component';

@Component({
    selector: 'skill-list',
    templateUrl: './skill-list.component.html',
    imports: [MatFormField, MatLabel, MatInput, FormsModule, MatCard, SkillComponent]
})
export class SkillListComponent implements OnInit {
    public skills: Skill[];
    public filter?: string;

    constructor(
        private readonly skillService: SkillService,
    ) {
    }

    isFiltered(skill: Skill): boolean {
        if (!this.filter) {
            return true;
        }
        let cleanFilter = removeDiacritics(this.filter).toLowerCase();
        return removeDiacritics(skill.name).toLowerCase().indexOf(cleanFilter) > -1;
    }

    getSkills() {
        this.skillService.getSkills().subscribe(
            skills => {
                this.skills = skills;
            }
        );
    }

    ngOnInit() {
        this.getSkills();
    }
}

