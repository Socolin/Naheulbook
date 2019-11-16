import {Component, OnInit, Input} from '@angular/core';

import {Skill} from './skill.model';
import {SkillService} from './skill.service';
import {Guid} from '../api/shared/util';

@Component({
    selector: 'skill-modifiers-editor',
    styleUrls: ['./skill-modifiers-editor.component.scss'],
    templateUrl: './skill-modifiers-editor.component.html',
})
export class SkillModifiersEditorComponent implements OnInit {
    @Input() modifiers: any[];
    public selectedSkillId?: Guid;
    public value?: number;
    public skills: Skill[];

    constructor(
        private readonly skillService: SkillService
    ) {
        this.modifiers = [];
    }

    removeModifier(i: number) {
        this.modifiers.splice(i, 1);
    }

    clear() {
        this.modifiers = [];
    }

    addModifier() {
        if (this.value && this.selectedSkillId) {
            let skill: Skill | undefined;
            for (let i = 0; i < this.skills.length; i++) {
                let s = this.skills[i];
                if (s.id === this.selectedSkillId) {
                    skill = s;
                    break;
                }
            }
            if (skill) {
                if (this.modifiers == null) {
                    this.modifiers = [];
                }
                this.modifiers.push({skill: skill, value: this.value});
                this.value = undefined;
                this.selectedSkillId = undefined;
            }
        }
        return true;
    }

    ngOnInit() {
        this.skillService.getSkills().subscribe(
            skills => {
                this.skills = skills;
            },
            err => {
                console.log(err);
            }
        );
    }
}
