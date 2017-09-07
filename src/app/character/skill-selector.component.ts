import {Component, EventEmitter, Input, Output, OnInit} from '@angular/core';

import {getRandomInt} from '../shared';

import {Job} from '../job';
import {Origin} from '../origin';

import {Skill, SkillService} from '../skill';
import {FlagData} from '../shared/flag.model';

@Component({
    selector: 'skill-selector',
    templateUrl: './skill-selector.component.html',
    styleUrls: ['./skill-selector.component.scss'],
})
export class SkillSelectorComponent implements OnInit {
    // Inputs
    @Input() selectedJob: Job | undefined;
    @Input() selectedOrigin: Origin;
    @Input() knownSkills: Skill[];
    @Input() skillCount = 2;

    // Outputs
    @Output() skillsSelected: EventEmitter<Skill[]> = new EventEmitter<Skill[]>();

    public selectedSkills: Skill[] = [];
    public skills: Skill[];

    constructor(private _skillService: SkillService) {
    }

    isSelected(skill: Skill) {
        for (let i = 0; i < this.selectedSkills.length; i++) {
            if (this.selectedSkills[i].id === skill.id) {
                return true;
            }
        }
        return false;
    }

    selectSkill(skill: Skill) {
        if (!this.isSelected(skill)) {
            if (this.selectedSkills.length === this.skillCount) {
                this.selectedSkills.splice(0, 1);
            }
            this.selectedSkills.push(skill);
            if (this.selectedSkills.length === this.skillCount) {
                this.skillsSelected.emit(this.selectedSkills);
            }
        } else {
            this.unselectSkill(skill);
        }
        return false;
    }

    unselectSkill(skill: Skill) {
        let index = this.selectedSkills.indexOf(skill);
        if (index !== -1) {
            this.selectedSkills.splice(index, 1);
        }
    }

    getSkills() {
        this._skillService.getSkills().subscribe(tmpSkills => {
            let availableSkills: number[] = [];

            if (!this.selectedJob) {
                if (this.selectedOrigin && this.selectedOrigin.availableSkills) {
                    for (let i = 0; i < this.selectedOrigin.availableSkills.length; i++) {
                        let skill = this.selectedOrigin.availableSkills[i];
                        availableSkills.push(skill.id);
                    }
                }
            }
            else {
                if (this.selectedJob.availableSkills) {
                    for (let i = 0; i < this.selectedJob.availableSkills.length; i++) {
                        let skill = this.selectedJob.availableSkills[i];
                        if (availableSkills.indexOf(skill.id) === -1) {
                            availableSkills.push(skill.id);
                        }
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

            let flagsData: {[flagName: string]: FlagData[]} = {};
            if (this.selectedJob) {
                this.selectedJob.getFlagsDatas(flagsData);
            }
            if (this.selectedOrigin) {
                this.selectedOrigin.getFlagsDatas(flagsData);
            }

            let skills: Skill[] = [];
            for (let i = 0; i < tmpSkills.length; i++) {
                let skill = tmpSkills[i];

                if (availableSkills.indexOf(skill.id) === -1) {
                    continue;
                }

                let ignoreSkill = false;
                if ('NO_SKILL' in flagsData) {
                    let noSkills = flagsData['NO_SKILL'];
                    for (let noSkill of noSkills) {
                        if (skill.hasFlag(noSkill.data)) {
                            ignoreSkill = true;
                        }
                    }
                }

                if (!ignoreSkill) {
                    skills.push(skill);
                }
            }

            this.skills = skills;
        });
    }

    randomSelect(): void {
        while (this.selectedSkills.length < this.skillCount) {
            let rnd = getRandomInt(0, this.skills.length - 1);
            let skill = this.skills[rnd];
            if (!this.isSelected(skill)) {
                this.selectSkill(skill);
            }
        }
    }

    ngOnInit() {
        this.getSkills();
    }
}
