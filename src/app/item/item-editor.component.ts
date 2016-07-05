import {Component, Input, OnInit, OnChanges} from '@angular/core';
import {NotificationsService} from '../notifications';

import {ItemTemplate, ItemSection, ItemSlot} from "../item";
import {Effect, EffectService} from "../effect";
import {IMetadata, ModifiersEditorComponent, StatRequirementsEditorComponent} from "../shared";
import {Skill, SkillService, SkillModifiersEditorComponent} from "../skill";
import {ItemElementComponent} from "./item-element.component";
import {ItemService} from "./item.service";

@Component({
    moduleId: module.id,
    selector: 'item-editor',
    templateUrl: 'item-editor.component.html',
    directives: [ModifiersEditorComponent, ItemElementComponent, SkillModifiersEditorComponent, StatRequirementsEditorComponent]
})
export class ItemEditorComponent implements OnInit, OnChanges {
    @Input() item: ItemTemplate;

    public skills: Skill[] = [];
    public sections: ItemSection[];
    public selectedSection: ItemSection;
    public slots: ItemSlot[];
    public form: {levels: number[], protection: number[], damage: number[], dices: number[]};

    private filteredEffects: Effect[];

    constructor(private _itemService: ItemService
        , private _effectService: EffectService
        , private _notification: NotificationsService
        , private _skillService: SkillService) {
        this.item = new ItemTemplate();
        this.form = {
            levels: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
            protection: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
            damage: [-2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
            dices: [1, 2, 3, 4, 5, 6]
        };
    }

    hasSpecial(token: string) {
        if (this.selectedSection) {
            if (this.selectedSection.special.indexOf(token) !== -1) {
                return true;
            }
        }
        return false;
    }

    getSkillById(skillId: number): IMetadata {
        for (let i = 0; i < this.skills.length; i++) {
            let skill = this.skills[i];
            if (skill.id === skillId) {
                return skill;
            }
        }
        return null;
    }

    addSkill(skillId: number) {
        let skill = this.getSkillById(skillId);
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
        let skill = this.getSkillById(skillId);
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
        if (this.item.slots.length === 0) {
            this.item.slotCount = null;
        } else {
            if (!this.item.slotCount) {
                this.item.slotCount = 1;
            }
        }
    }

    searchEffect(filterName) {
        this._effectService.searchEffect(filterName).subscribe(
            effects => {
                this.filteredEffects = effects;
            },
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
    }

    ngOnChanges() {
        if (this.item.category && this.sections) {
            for (let i = 0; i < this.sections.length; i++) {
                let t = this.sections[i];
                if (t.id === this.item.category.type) {
                    this.selectedSection = t;
                    break;
                }
            }
        }
        if (this.selectedSection) {
            if (!this.hasSpecial('CAN_HAVE_MIN_LEVEL')) {
                this.item.level = null;
            }
            if (!this.hasSpecial('CAN_BE_THROWABLE')) {
                this.item.throwable = null;
            }
            if (!this.hasSpecial('CAN_HAVE_RUPTURE')) {
                this.item.rupture = null;
            }
            if (!this.hasSpecial('HAVE_DAMAGE')) {
                this.item.damageDiceCount = null;
                this.item.damageType = null;
                this.item.damage = null;
            }
            if (!this.hasSpecial('HAVE_PROTECTION')) {
                this.item.magicProtection = null;
                this.item.protection = null;
                this.item.protectionAgainstMagic = null;
                this.item.protectionAgainstType = null;
            }
            if (!this.hasSpecial('CAN_HAVE_MODIFIER')) {
                this.item.modifiers = null;
                this.item.modifiers = [];
            } else {
                if (this.item.modifiers == null) {
                    this.item.modifiers = [];
                }
            }
            if (!this.hasSpecial('DICE_DROP')) {
                this.item.diceDrop = null;
            }
            if (!this.hasSpecial('HAVE_CHARGE')) {
                this.item.charge = null;
            }
        }
    }

    ngOnInit() {
        this._itemService.getSectionsList().subscribe(sections => {
            this.sections = sections;
            this.ngOnChanges();
        });
        this._skillService.getSkills().subscribe(res => {
            this.skills = res;
        });
        this._itemService.getSlots().subscribe(res => {
            this.slots = res;
        });
    }
}
