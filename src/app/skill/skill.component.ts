import {Component} from '@angular/core';

import {Skill} from "./skill.model";

@Component({
    selector: 'skill',
    templateUrl: 'app/skill/skill.component.html',
    styleUrls: ['/styles/skill.css'],
    inputs: ['skill']
})
export class SkillComponent {
    public skill: Skill;
}
