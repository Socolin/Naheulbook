import {Component, EventEmitter} from '@angular/core';

import {SkillComponent} from './skill.component';

import {Job} from '../job';
import {Origin} from '../origin';
import {Skill} from "./skill.model";
import {SkillService} from "./skill.service";

@Component({
    selector: 'skill-selector',
    templateUrl: 'app/skill/skill-selector.component.html',
    inputs: ['selectedJob', 'selectedOrigin', 'knownSkills', 'skillCount'],
    outputs: ['skillsSelected'],
    directives: [SkillComponent]
})
export class SkillSelectorComponent {
    // Inputs
    public selectedJob: Job;
    public selectedOrigin: Origin;
    public knownSkills: Skill[];
    public skillCount: number;

    // Outputs
    private skillsSelected: EventEmitter<Skill[]> = new EventEmitter<Skill[]>();

    public selectedSkills: Skill[];
    public skills: Skill[];

    constructor(private _skillService: SkillService) {
        this.selectedSkills = [];
        this.skillCount = 2;
    }

    isSelected(skill: Skill) {
        for (var i = 0; i < this.selectedSkills.length; i++) {
            if (this.selectedSkills[i].id == skill.id) {
                return true;
            }
        }
        return false;
    }

    selectSkill(skill: Skill) {
        if (!this.isSelected(skill)) {
            if (this.selectedSkills.length == this.skillCount) {
                this.selectedSkills.splice(0, 1);
            }
            this.selectedSkills.push(skill);
            if (this.selectedSkills.length == this.skillCount) {
                this.skillsSelected.emit(this.selectedSkills);
            }
        } else {
            this.unselectSkill(skill);
        }
        return false;
    }

    unselectSkill(skill: Skill) {
        var index = this.selectedSkills.indexOf(skill);
        if (index != -1) {
            this.selectedSkills.splice(index, 1);
        }
    }

    getSkills() {
        var ignoreOrigin = false;
        for (var i = 0; i < this.selectedOrigin.specials.length; i++) {
            if (this.selectedOrigin.specials[i] == 'USE_JOB_SKILLS') {
                if (this.selectedJob != null) {
                    ignoreOrigin = true;
                }
                break;
            }
        }

        this._skillService.getSkills().subscribe(tmpSkills => {
            var availableSkills = [];

            if (!ignoreOrigin) {
                if (this.selectedOrigin && this.selectedOrigin.availableSkills) {
                    for (var i = 0; i < this.selectedOrigin.availableSkills.length; i++) {
                        var skill = this.selectedOrigin.availableSkills[i];
                        availableSkills.push(skill.id);
                    }
                }
            }

            if (this.selectedJob && this.selectedJob.availableSkills) {
                for (var i = 0; i < this.selectedJob.availableSkills.length; i++) {
                    var skill = this.selectedJob.availableSkills[i];
                    if (availableSkills.indexOf(skill.id) == -1) {
                        availableSkills.push(skill.id);
                    }
                }
            }

            if (!ignoreOrigin) {
                if (this.selectedOrigin && this.selectedOrigin.skills) {
                    for (var i = 0; i < this.selectedOrigin.skills.length; i++) {
                        var skill = this.selectedOrigin.skills[i];
                        var index = availableSkills.indexOf(skill.id);
                        if (index >= 0) {
                            availableSkills.splice(index, 1);
                        }
                    }
                }
            }

            if (this.selectedJob && this.selectedJob.skills) {
                for (let i = 0; i < this.selectedJob.skills.length; i++) {
                    let skill = this.selectedJob.skills[i];
                    let index = availableSkills.indexOf(skill.id);
                    if (index >= 0) {
                        availableSkills.splice(index, 1);
                    }
                }
            }

            if (this.knownSkills) {
                for (let i = 0; i < this.knownSkills.length; i++) {
                    let skill = this.knownSkills[i];
                    let index = availableSkills.indexOf(skill.id);
                    if (index >= 0) {
                        availableSkills.splice(index, 1);
                    }
                }
            }

            let skills = [];
            for (let i = 0; i < tmpSkills.length; i++) {
                let skill = tmpSkills[i];
                if (availableSkills.indexOf(skill.id) != -1) {
                    skills.push(skill);
                }
            }
            this.skills = skills;
        });
    }

    ngOnInit() {
        this.getSkills();
    }
}
