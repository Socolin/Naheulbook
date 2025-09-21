import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import { Portal, CdkPortal } from '@angular/cdk/portal';

import {God, NhbkDialogService, removeDiacritics} from '../shared';
import {Skill} from '../skill';
import {NhbkAction, NhbkActionEditorDialogComponent, NhbkActionEditorDialogData} from '../action';
import {LoginService} from '../user';

import {ItemSlot, ItemTemplate} from './item-template.model';
import {ItemTemplateService} from './item-template.service';
import {itemTemplateModulesDefinitions} from './item-template-modules-definitions';
import {ItemTypeResponse} from '../api/responses';
import {NhbkMatDialog} from '../material-workaround';
import { MatCard, MatCardHeader, MatCardTitle, MatCardContent, MatCardSubtitle, MatCardActions } from '@angular/material/card';
import { NgSwitch, NgSwitchCase, NgFor, NgIf, NgSwitchDefault } from '@angular/common';
import { MatIconButton, MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField, MatLabel, MatSuffix, MatPrefix } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { NhbkActionComponent } from '../action/nhbk-action.component';
import { MatMenuTrigger, MatMenu, MatMenuItem } from '@angular/material/menu';
import { MatSelect } from '@angular/material/select';
import { MatOption } from '@angular/material/autocomplete';
import { MatDivider } from '@angular/material/list';
import { MatCheckbox } from '@angular/material/checkbox';
import { DurationSelectorComponent } from '../date/duration-selector.component';
import { ModifiersEditorComponent } from '../effect/modifiers-editor.component';
import { StatRequirementsEditorComponent } from '../shared/stat-requirements-editor.component';
import { MatRadioGroup, MatRadioButton } from '@angular/material/radio';
import { SkillModifiersEditorComponent } from '../skill/skill-modifiers-editor.component';
import { MatExpansionPanelActionRow } from '@angular/material/expansion';

@Component({
    selector: 'item-template-editor-module',
    styleUrls: ['./item-template-editor-module.component.scss'],
    templateUrl: './item-template-editor-module.component.html',
    imports: [MatCard, NgSwitch, MatIconButton, MatIcon, NgSwitchCase, MatCardHeader, MatCardTitle, MatCardContent, MatFormField, MatLabel, MatInput, FormsModule, NgFor, NgIf, NhbkActionComponent, MatMenuTrigger, MatMenu, MatMenuItem, MatButton, MatSuffix, MatSelect, MatOption, MatDivider, MatCardSubtitle, MatCheckbox, MatCardActions, DurationSelectorComponent, ModifiersEditorComponent, StatRequirementsEditorComponent, MatRadioGroup, MatRadioButton, SkillModifiersEditorComponent, MatPrefix, NgSwitchDefault, CdkPortal, MatExpansionPanelActionRow]
})
export class ItemTemplateEditorModuleComponent implements OnInit {
    @Input() itemTemplate: ItemTemplate;
    @Input() moduleName: string;

    @Input() slots: ItemSlot[];
    @Input() itemTypes: ItemTypeResponse[];
    @Input() gods: God[];
    @Input() skills: Skill[] = [];
    @Input() skillsById: { [skillId: number]: Skill } = {};

    @Output() deleted: EventEmitter<any> = new EventEmitter<any>();

    @ViewChild('createItemTypeDialog')
    public createItemTypeDialog: Portal<any>;
    public createItemTypeOverlayRef?: OverlayRef;
    public newItemTypeDisplayName = '';
    public newItemTypeTechName = '';
    public moduleDefinitionByName = itemTemplateModulesDefinitions.reduce((result, value) => {
        result[value.name] = value;
        return result;
    }, {});

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly nhbkDialogService: NhbkDialogService,
        public readonly loginService: LoginService,
        private readonly itemTemplateService: ItemTemplateService,
    ) {
    }

    openEditChargeActionModal(action?: NhbkAction) {
        const dialogRef = this.dialog.open<NhbkActionEditorDialogComponent, NhbkActionEditorDialogData, NhbkAction>(
            NhbkActionEditorDialogComponent,
            {data: {action}}
        );
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }
            if (action && this.itemTemplate.data.actions) {
                const index = this.itemTemplate.data.actions.findIndex(a => a === action);
                this.itemTemplate.data.actions[index] = result;
            } else {
                this.addChargeAction(result);
            }
        });
    }

    addChargeAction(action: NhbkAction) {
        if (this.itemTemplate.data.actions === undefined) {
            this.itemTemplate.data.actions = [];
        }
        this.itemTemplate.data.actions.push(action);
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
        this.createItemTypeOverlayRef = this.nhbkDialogService.openCenteredBackdropDialog(this.createItemTypeDialog, true);
    }

    closeCreateItemTypeDialog() {
        if (this.createItemTypeOverlayRef) {
            this.createItemTypeOverlayRef.detach();
            this.createItemTypeOverlayRef = undefined;
        }
    }

    public createItemType() {
        this.itemTemplateService.createItemType(this.newItemTypeDisplayName, this.newItemTypeTechName)
            .subscribe(itemType => {
                this.itemTypes.push(itemType);
            });
        this.newItemTypeDisplayName = '';
        this.newItemTypeTechName = '';
        this.closeCreateItemTypeDialog();
    }

    deleteModule() {
        this.deleted.emit(true);
    }

    ngOnInit(): void {

    }
}
