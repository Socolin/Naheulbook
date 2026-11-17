import {Component, Input, ChangeDetectionStrategy} from '@angular/core';

import {Skill} from './skill.model';
import { MatCardTitle, MatCardHeader, MatCardContent } from '@angular/material/card';
import { NgIf } from '@angular/common';

@Component({
    selector: 'skill',
    templateUrl: './skill.component.html',
    styleUrls: ['./skill.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [MatCardTitle, MatCardHeader, NgIf, MatCardContent]
})
export class SkillComponent {
    @Input() skill: Skill;
}
