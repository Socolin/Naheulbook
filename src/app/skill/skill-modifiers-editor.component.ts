import {Component, OnInit, Input} from '@angular/core';

import {Skill} from './skill.model';
import {SkillService} from './skill.service';

@Component({
    selector: 'skill-modifiers-editor',
    templateUrl: './skill-modifiers-editor.component.html',
})
export class SkillModifiersEditorComponent implements OnInit {
    @Input() modifiers: any[];
    public selectedSkill: number;
    public value: number;
    public skills: Skill[];

    constructor(private _skillService: SkillService) {
        this.modifiers = [];
    }

    setValue(value) {
        this.value = value;
        this.addModifier();
    }

    removeModifier(i: number) {
        this.modifiers.splice(i, 1);
    }

    clear() {
        this.modifiers = [];
    }

    addModifier() {
        if (this.value && this.selectedSkill) {
            let skill = null;
            for (let i = 0; i < this.skills.length; i++) {
                let s = this.skills[i];
                if (s.id === this.selectedSkill) {
                    skill = s;
                    break;
                }
            }
            if (skill) {
                if (this.modifiers == null) {
                    this.modifiers = [];
                }
                this.modifiers.push({skill: skill, value: this.value});
                this.value = null;
                this.selectedSkill = null;
            }
        }
        return true;
    }

    ngOnInit() {
        this._skillService.getSkills().subscribe(
            skills => {
                this.skills = skills;
            },
            err => {
                console.log(err);
            }
        );
    }
}
