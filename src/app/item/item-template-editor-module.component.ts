import {Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import {ItemTemplate, ItemSlot} from './item-template.model';
import {Skill} from '../skill/skill.model';
import {NhbkDateOffset} from '../date/date.model';
import {dateOffset2TimeDuration} from '../date/util';

@Component({
    selector: 'item-template-editor-module',
    styleUrls: ['./item-template-editor-module.component.scss'],
    templateUrl: './item-template-editor-module.component.html'
})
export class ItemTemplateEditorModuleComponent implements OnInit {
    @Input() itemTemplate: ItemTemplate;
    @Input() moduleName: string;

    @Input() slots: ItemSlot[];
    @Input() skills: Skill[] = [];
    @Input() skillsById: { [skillId: number]: Skill } = {};

    @Output() onDelete: EventEmitter<any> = new EventEmitter<any>();

    public lifetimeDateOffset: NhbkDateOffset = new NhbkDateOffset();

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


    deleteModule() {
        this.onDelete.emit(true);
    }

    ngOnInit(): void {

    }
}
