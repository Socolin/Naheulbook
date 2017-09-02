import {Component, OnInit, Input, Output, EventEmitter, ViewChild} from '@angular/core';
import {OverlayRef, Portal} from '@angular/material';

import {ItemTemplate, ItemSlot, ItemType} from './item-template.model';
import {Skill} from '../skill/skill.model';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';
import {NhbkAction} from '../action/nhbk-action.model';
import {removeDiacritics} from '../shared/remove_diacritics';
import {ItemTemplateService} from './item-template.service';

@Component({
    selector: 'item-template-editor-module',
    styleUrls: ['./item-template-editor-module.component.scss'],
    templateUrl: './item-template-editor-module.component.html'
})
export class ItemTemplateEditorModuleComponent implements OnInit {
    @Input() itemTemplate: ItemTemplate;
    @Input() moduleName: string;

    @Input() slots: ItemSlot[];
    @Input() itemTypes: ItemType[];
    @Input() skills: Skill[] = [];
    @Input() skillsById: { [skillId: number]: Skill } = {};

    @Output() onDelete: EventEmitter<any> = new EventEmitter<any>();

    @ViewChild('addChargeActionModal')
    public addChargeActionModal: Portal<any>;
    public addChargeActionOverlayRef: OverlayRef;

    @ViewChild('createItemTypeDialog')
    public createItemTypeDialog: Portal<any>;
    public createItemTypeOverlayRef?: OverlayRef;
    public newItemTypeDisplayName = '';
    public newItemTypeTechName = '';

    constructor(private _nhbkDialogService: NhbkDialogService,
                private _itemTemplateService: ItemTemplateService) {
    }

    openAddChargeActionModal() {
        if (!this.itemTemplate.data.actions) {
            this.itemTemplate.data.actions = [];
        }
        this.addChargeActionOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.addChargeActionModal);
    }

    closeAddChargeActionModal() {
        this.addChargeActionOverlayRef.detach();
    }

    addChargeAction(action: NhbkAction) {
        this.itemTemplate.data.actions!.push(action);
        this.closeAddChargeActionModal();
    }

    removeChargeAction(index: number) {
        this.itemTemplate.data.actions!.splice(index, 1);
    }

    updateNewItemTypeDisplayName(newName: string) {
        let previousTechName = removeDiacritics(this.newItemTypeDisplayName)
            .replace(' ', '_')
            .toUpperCase();
        if (previousTechName === this.newItemTypeTechName) {
            this.newItemTypeTechName = removeDiacritics(newName)
                .replace(' ', '_')
                .toUpperCase();
        }
        this.newItemTypeDisplayName = newName;
    }

    openCreateItemTypeDialog() {
        this.createItemTypeOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.createItemTypeDialog, true);
    }

    closeCreateItemTypeDialog() {
        if (this.createItemTypeOverlayRef) {
            this.createItemTypeOverlayRef.detach();
            this.createItemTypeOverlayRef = undefined;
        }
    }

    public createItemType() {
        this._itemTemplateService.createItemType(this.newItemTypeDisplayName, this.newItemTypeTechName)
            .subscribe(itemType => {
                this.itemTypes.push(itemType);
            });
        this.newItemTypeDisplayName = '';
        this.newItemTypeTechName = '';
        this.closeCreateItemTypeDialog();
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

    deleteModule() {
        this.onDelete.emit(true);
    }

    ngOnInit(): void {

    }
}
