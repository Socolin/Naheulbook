import {Component, Input, OnInit, OnChanges, SimpleChanges} from '@angular/core';

import {ItemTemplate, ItemSection, ItemSlot} from '../item';
import {Effect, EffectService} from '../effect';
import {Skill, SkillService} from '../skill';
import {ItemService} from './item.service';
import {Observable} from 'rxjs';
import {JobService} from '../job/job.service';
import {OriginService} from '../origin/origin.service';
import {NhbkDateOffset} from '../date/date.model';
import {dateOffset2TimeDuration} from '../date/util';
import {isNullOrUndefined} from 'util';

@Component({
    selector: 'item-template-editor',
    styleUrls: ['./item-template-editor.component.scss'],
    templateUrl: './item-template-editor.component.html'
})
export class ItemTemplateEditorComponent implements OnInit, OnChanges {
    @Input() itemTemplate: ItemTemplate;

    public modules: string[] = [];

    // Datas usefull for forms
    public skills: Skill[] = [];
    public skillsById: { [skillId: number]: Skill } = {};
    public sections: ItemSection[];
    public slots: ItemSlot[];
    public originsName: { [originId: number]: string };
    public jobsName: { [jobId: number]: string };

    public lifetimeDateOffset: NhbkDateOffset = new NhbkDateOffset();
    public selectedSection: ItemSection;
    public form: { levels: number[], protection: number[], damage: number[], dices: number[] };

    public filteredEffects: Effect[];

    constructor(private _itemService: ItemService
        , private _effectService: EffectService
        , private _originService: OriginService
        , private _jobService: JobService
        , private _skillService: SkillService) {
        this.itemTemplate = new ItemTemplate();
        this.form = {
            levels: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
            protection: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
            damage: [-2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
            dices: [1, 2, 3, 4, 5, 6]
        };
    }

    addSkill(skillId: number) {
        let skill = this.skillsById[skillId];
        if (!this.itemTemplate.skills) {
            this.itemTemplate.skills = [];
        }
        this.itemTemplate.skills.push(skill);
    }

    removeSkill(skillId: number) {
        if (this.itemTemplate.skills) {
            for (let i = 0; i < this.itemTemplate.skills.length; i++) {
                let skill = this.itemTemplate.skills[i];
                if (skill.id === skillId) {
                    this.itemTemplate.skills.splice(i, 1);
                    break;
                }
            }
        }
    }

    addUnskill(skillId: number) {
        let skill = this.skillsById[skillId];
        if (!this.itemTemplate.unskills) {
            this.itemTemplate.unskills = [];
        }
        this.itemTemplate.unskills.push(skill);
    }

    removeUnskill(skillId: number) {
        if (this.itemTemplate.unskills) {
            for (let i = 0; i < this.itemTemplate.unskills.length; i++) {
                let skill = this.itemTemplate.unskills[i];
                if (skill.id === skillId) {
                    this.itemTemplate.unskills.splice(i, 1);
                    break;
                }
            }
        }
    }

    isInSlot(slot) {
        if (!this.itemTemplate.slots) {
            return false;
        }
        for (let i = 0; i < this.itemTemplate.slots.length; i++) {
            if (this.itemTemplate.slots[i].id === slot.id) {
                return true;
            }
        }
        return false;
    }

    toggleSlot(slot) {
        if (!this.itemTemplate.slots) {
            this.itemTemplate.slots = [];
        }
        if (this.isInSlot(slot)) {
            for (let i = 0; i < this.itemTemplate.slots.length; i++) {
                if (this.itemTemplate.slots[i].id === slot.id) {
                    this.itemTemplate.slots.splice(i, 1);
                    break;
                }
            }
        } else {
            this.itemTemplate.slots.push(slot);
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
        [this.itemTemplate.data.notIdentifiedName] = this.itemTemplate.name.split(' ');
    }

    setRupture(rupture) {
        this.itemTemplate.data.rupture = rupture;
    }

    updateSelectedSection() {
        if (this.itemTemplate && this.sections) {
            for (let i = 0; i < this.sections.length; i++) {
                let section = this.sections[i];
                for (let j = 0; j < section.categories.length; j++) {
                    let category = section.categories[j];
                    if (category.id === this.itemTemplate.category) {
                        this.selectedSection = section;
                        break;
                    }
                }
            }
        }
    }

    setItemLifetimeDateOffset(dateOffset: NhbkDateOffset) {
        this.itemTemplate.data.lifetime = dateOffset2TimeDuration(dateOffset);
    }

    setLifetimeType(type: string) {
        if (type === null) {
            this.itemTemplate.data.lifetime = null;
            this.itemTemplate.data.lifetimeType = null;
        } else {
            this.itemTemplate.data.lifetimeType = type;
            if (type === 'combat' || type === 'lap') {
                this.itemTemplate.data.lifetime = 1;
            }
            else if (type === 'time') {
                if (this.lifetimeDateOffset) {
                    this.setItemLifetimeDateOffset(this.lifetimeDateOffset);
                }
                else {
                    this.itemTemplate.data.lifetime = 0;
                }
            }
            else if (type === 'custom') {
                this.itemTemplate.data.lifetime = '';
            }
        }
    }

    ngOnChanges(changes: SimpleChanges): void {
        if ('itemTemplate' in changes) {
            this.updateSelectedSection();
        }
    }

    addModule(moduleName: string) {
        if (this.modules.indexOf(moduleName) !== -1) {
            return;
        }

        switch (moduleName) {
            case 'charge':
                this.itemTemplate.data.charge = 1;
                break;
            case 'container':
                this.itemTemplate.data.container = true;
                break;
            case 'damage':
                this.itemTemplate.data.damageDice = 1;
                break;
            case 'gem':
                this.itemTemplate.data.useUG = true;
                break;
            case 'level':
                this.itemTemplate.data.requireLevel = 1;
                break;
            case 'modifiers':
                this.itemTemplate.modifiers = [];
                break;
            case 'prereq':
                this.itemTemplate.requirements = [];
                break;
            case 'protection':
                this.itemTemplate.data.protection = 1;
                break;
            case 'relic':
                this.itemTemplate.data.relic = true;
                break;
            case 'rupture':
                this.itemTemplate.data.rupture = 1;
                break;
            case 'sex':
                this.itemTemplate.data.sex = 'h';
                break;
            case 'skill':
                this.itemTemplate.skills = [];
                this.itemTemplate.unskills = [];
                this.itemTemplate.skillModifiers = [];
                break;
            case 'slots':
                this.itemTemplate.slots = [];
                break;
        }

        this.modules.push(moduleName);
    }

    removeModule(moduleName: string) {
        let moduleIndex = this.modules.indexOf(moduleName);
        if (moduleIndex === -1) {
            return;
        }

        switch (moduleName) {
            case 'charge':
                this.itemTemplate.data.charge = null;
                break;
            case 'container':
                this.itemTemplate.data.container = null;
                break;
            case 'damage':
                this.itemTemplate.data.damageDice = null;
                this.itemTemplate.data.bonusDamage = null;
                this.itemTemplate.data.damageType = null;
                break;
            case 'gem':
                this.itemTemplate.data.useUG = null;
                break;
            case 'level':
                this.itemTemplate.data.requireLevel = null;
                break;
            case 'modifiers':
                this.itemTemplate.modifiers = null;
                break;
            case 'prereq':
                this.itemTemplate.requirements = null;
                break;
            case 'protection':
                this.itemTemplate.data.protection = null;
                this.itemTemplate.data.protectionAgainstMagic = null;
                this.itemTemplate.data.magicProtection = null;
                this.itemTemplate.data.protectionAgainstType = null;
                break;
            case 'relic':
                this.itemTemplate.data.relic = null;
                break;
            case 'rupture':
                this.itemTemplate.data.rupture = null;
                break;
            case 'sex':
                this.itemTemplate.data.sex = null;
                break;
            case 'skill':
                this.itemTemplate.skills = null;
                this.itemTemplate.unskills = null;
                this.itemTemplate.skillModifiers = null;
                break;
            case 'slots':
                this.itemTemplate.slots = null;
                break;
        }

        this.modules.splice(moduleIndex, 1);
    }

    determineModulesFromItemTemplate(): void {
        let modules: string[] = [];

        if (this.itemTemplate.data.charge) {
            modules.push('charge');
        }
        if (this.itemTemplate.data.container) {
            modules.push('container');
        }
        if (this.itemTemplate.data.damageDice || this.itemTemplate.data.bonusDamage) {
            modules.push('damage');
        }
        if (this.itemTemplate.data.useUG) {
            modules.push('gem');
        }
        if (this.itemTemplate.data.requireLevel) {
            modules.push('level');
        }
        if (this.itemTemplate.modifiers !== null && this.itemTemplate.modifiers.length) {
            modules.push('modifiers');
        }
        if (this.itemTemplate.requirements !== null
            && this.itemTemplate.requirements.length) {
            modules.push('prereq');
        }
        if (this.itemTemplate.data.protection
            || this.itemTemplate.data.magicProtection
            || this.itemTemplate.data.protectionAgainstMagic) {
            modules.push('protection');
        }
        if (this.itemTemplate.data.relic) {
            modules.push('relic');
        }
        if (!isNullOrUndefined(this.itemTemplate.data.rupture)) {
            modules.push('rupture');
        }
        if (this.itemTemplate.data.sex) {
            modules.push('sex');
        }
        if ((this.itemTemplate.skills !== null && this.itemTemplate.skills.length)
            || (this.itemTemplate.unskills !== null && this.itemTemplate.unskills.length)
            || (this.itemTemplate.skillModifiers !== null && this.itemTemplate.skillModifiers.length)) {
            modules.push('skill');
        }
        if (this.itemTemplate.slots !== null && this.itemTemplate.slots.length) {
            modules.push('slots');
        }

        this.modules = modules;
    }

    ngOnInit() {
        if (!this.itemTemplate) {
            this.itemTemplate = new ItemTemplate();
        } else {
            this.determineModulesFromItemTemplate();
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

            let jobsName: { [jobId: number]: string } = {};
            for (let i = 0; i < jobs.length; i++) {
                let job = jobs[i];
                jobsName[job.id] = job.name;
            }
            this.jobsName = jobsName;

            let originsName: { [originId: number]: string } = {};
            for (let i = 0; i < origins.length; i++) {
                let origin = origins[i];
                originsName[origin.id] = origin.name;
            }
            this.originsName = originsName;

            this.updateSelectedSection();
        });
    }
}
