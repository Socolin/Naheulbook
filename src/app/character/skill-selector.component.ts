import {Component, EventEmitter, Input, Output, OnInit} from '@angular/core';

import {FlagData, getRandomInt} from '../shared';

import {Job} from '../job';
import {Origin} from '../origin';

import {Skill, SkillService} from '../skill';
import { MatSelectionListChange } from '@angular/material/list';
import {Guid} from '../api/shared/util';

@Component({
    selector: 'skill-selector',
    templateUrl: './skill-selector.component.html',
    styleUrls: ['./skill-selector.component.scss'],
})
export class SkillSelectorComponent implements OnInit {
    // Inputs
    @Input() selectedJobs: Job[];
    @Input() selectedOrigin: Origin;
    @Input() knownSkills: Skill[];
    @Input() skillCount = 2;

    // Outputs
    @Output() skillsSelected: EventEmitter<Skill[]> = new EventEmitter<Skill[]>();

    public skills: Skill[];

    constructor(
        private readonly skillService: SkillService
    ) {
    }

    selectionChange(event: MatSelectionListChange) {
        if (event.source.selectedOptions.selected.length === this.skillCount) {
            this.skillsSelected.next(event.source.selectedOptions.selected.map(s => s.value));
        }
    }

    getSkills() {
        this.skillService.getSkills().subscribe(tmpSkills => {
            let availableSkills: Guid[] = [];

            if (!this.selectedJobs.length) {
                if (this.selectedOrigin && this.selectedOrigin.availableSkills) {
                    for (let i = 0; i < this.selectedOrigin.availableSkills.length; i++) {
                        let skill = this.selectedOrigin.availableSkills[i];
                        availableSkills.push(skill.id);
                    }
                }
            }
            else {
                for (let job of this.selectedJobs) {
                    if (job.availableSkills) {
                        for (let i = 0; i < job.availableSkills.length; i++) {
                            let skill = job.availableSkills[i];
                            if (availableSkills.indexOf(skill.id) === -1) {
                                availableSkills.push(skill.id);
                            }
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
            for (let job of this.selectedJobs) {
                job.getFlagsDatas(flagsData);
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
        const selectedSkills: Skill[] = [];
        while (selectedSkills.length < this.skillCount) {
            let rnd = getRandomInt(0, this.skills.length - 1);
            let skill = this.skills[rnd];
            if (selectedSkills.indexOf(skill) === -1) {
                selectedSkills.push(skill);
            }
        }
        this.skillsSelected.next(selectedSkills)
    }

    ngOnInit() {
        this.getSkills();
    }
}
