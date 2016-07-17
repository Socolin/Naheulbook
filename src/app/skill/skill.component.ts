import {Component, Input} from '@angular/core';

import {Skill} from "./skill.model";

@Component({
    selector: 'skill',
    moduleId: module.id,
    templateUrl: 'skill.component.html'
})
export class SkillComponent {
    @Input() skill: Skill;
}
