import {Component, Input, OnInit, OnChanges, SimpleChanges} from '@angular/core';
import {NotificationsService} from '../notifications';

import {ItemTemplate, ItemSection, ItemSlot} from "../item";
import {Effect, EffectService} from "../effect";
import {Skill, SkillService} from "../skill";
import {ItemService} from "./item.service";
import {Observable} from "rxjs";
import {JobService} from "../job/job.service";
import {OriginService} from "../origin/origin.service";

@Component({
    selector: 'item-editor',
    templateUrl: 'item-editor.component.html'
})
export class ItemEditorComponent implements OnInit, OnChanges {
    @Input() item: ItemTemplate;

    public skills: Skill[] = [];
    public skillsById: {[skillId: number]: Skill} = {};
    public sections: ItemSection[];
    public selectedSection: ItemSection;
    public slots: ItemSlot[];
    public form: {levels: number[], protection: number[], damage: number[], dices: number[]};
    private originsName: {[originId: number]: string};
    private jobsName: {[jobId: number]: string};

    private filteredEffects: Effect[];

    constructor(private _itemService: ItemService
        , private _effectService: EffectService
        , private _notification: NotificationsService
        , private _originService: OriginService
        , private _jobService: JobService
        , private _skillService: SkillService) {
        this.item = new ItemTemplate();
        this.form = {
            levels: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
            protection: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
            damage: [-2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
            dices: [1, 2, 3, 4, 5, 6]
        };
    }

    addSkill(skillId: number) {
        let skill = this.skillsById[skillId];
        if (!this.item.skills) {
            this.item.skills = [];
        }
        this.item.skills.push(skill);
    }

    removeSkill(skillId: number) {
        if (this.item.skills) {
            for (let i = 0; i < this.item.skills.length; i++) {
                let skill = this.item.skills[i];
                if (skill.id === skillId) {
                    this.item.skills.splice(i, 1);
                    break;
                }
            }
        }
    }

    addUnskill(skillId: number) {
        let skill = this.skillsById[skillId];
        if (!this.item.unskills) {
            this.item.unskills = [];
        }
        this.item.unskills.push(skill);
    }

    removeUnskill(skillId: number) {
        if (this.item.unskills) {
            for (let i = 0; i < this.item.unskills.length; i++) {
                let skill = this.item.unskills[i];
                if (skill.id === skillId) {
                    this.item.unskills.splice(i, 1);
                    break;
                }
            }
        }
    }

    isInSlot(slot) {
        if (!this.item.slots) {
            return false;
        }
        for (let i = 0; i < this.item.slots.length; i++) {
            if (this.item.slots[i].id === slot.id) {
                return true;
            }
        }
        return false;
    }

    toggleSlot(slot) {
        if (!this.item.slots) {
            this.item.slots = [];
        }
        if (this.isInSlot(slot)) {
            for (let i = 0; i < this.item.slots.length; i++) {
                if (this.item.slots[i].id === slot.id) {
                    this.item.slots.splice(i, 1);
                    break;
                }
            }
        } else {
            this.item.slots.push(slot);
        }
    }

    searchEffect(filterName) {
        this._effectService.searchEffect(filterName).subscribe(
            effects => {
                this.filteredEffects = effects;
            }
        );
    }

    automaticNotIdentifiedName() {
        [this.item.data.notIdentifiedName] = this.item.name.split(' ');
    }

    setRupture(rupture) {
        this.item.data.rupture = rupture;
    }

    updateSelectedSection() {
        if (this.item && this.sections) {
            for (let i = 0; i < this.sections.length; i++) {
                let section = this.sections[i];
                for (let j = 0; j < section.categories.length; j++) {
                    let category = section.categories[j];
                    if (category.id === this.item.category) {
                        this.selectedSection = section;
                        break;
                    }
                }
            }
        }
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ('item' in changes) {
            this.updateSelectedSection();
        }
    }

    ngOnInit() {
        if (!this.item) {
            this.item = new ItemTemplate();
        }
        Observable.forkJoin(
            this._itemService.getSectionsList(),
            this._skillService.getSkills(),
            this._skillService.getSkillsById(),
            this._itemService.getSlots(),
            this._jobService.getJobList(),
            this._originService.getOriginList()
        ).subscribe(([sections, skills, skillsById, slots, jobs, origins]) => {
            this.sections = sections;
            this.skills = skills;
            this.skillsById = skillsById;
            this.slots = slots;

            let jobsName: {[jobId: number]: string} = {};
            for (let i = 0; i < jobs.length; i++) {
                let job = jobs[i];
                jobsName[job.id] = job.name;
            }
            this.jobsName = jobsName;

            let originsName: {[originId: number]: string} = {};
            for (let i = 0; i < origins.length; i++) {
                let origin = origins[i];
                originsName[origin.id] = origin.name;
            }
            this.originsName = originsName;

            this.updateSelectedSection();
        });
    }
}
