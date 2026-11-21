import {Component, Input, ChangeDetectionStrategy} from '@angular/core';

import {Skill} from './skill.model';
import {ModifierPipe} from '../shared';

export interface SkillSource {
    name: string;
    /** true when the skill is granted by default, false when it can be picked */
    base: boolean;
}

export interface SkillSources {
    origins: SkillSource[];
    jobs: SkillSource[];
}

@Component({
    selector: 'skill',
    templateUrl: './skill.component.html',
    styleUrls: ['./skill.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [ModifierPipe]
})
export class SkillComponent {
    @Input() skill: Skill;
    @Input() sources?: SkillSources;
}
