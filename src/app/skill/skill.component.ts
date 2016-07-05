import {Component, Input} from '@angular/core';

import {Skill} from "./skill.model";

@Component({
    selector: 'skill',
    templateUrl: 'app/skill/skill.component.html'
})
export class SkillComponent {
    @Input() skill: Skill;
}
