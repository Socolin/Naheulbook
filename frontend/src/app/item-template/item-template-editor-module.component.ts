import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import {CdkPortal, Portal} from '@angular/cdk/portal';

import {God, NhbkDialogService, removeDiacritics, StatRequirementsEditorComponent} from '../shared';
import {Skill, SkillModifiersEditorComponent} from '../skill';
import {NhbkAction} from '../action';
import {LoginService} from '../user';

import {ItemSlot, ItemTemplate} from './item-template.model';
import {ItemTemplateService} from './item-template.service';
import {itemTemplateModulesDefinitions} from './item-template-modules-definitions';
import {ItemTypeResponse} from '../api/responses';
import {NhbkMatDialog} from '../material-workaround';
import {MatCard, MatCardActions, MatCardContent, MatCardHeader, MatCardSubtitle, MatCardTitle} from '@angular/material/card';
import {NgFor, NgIf, NgSwitch, NgSwitchCase, NgSwitchDefault} from '@angular/common';
import {MatButton, MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatFormField, MatLabel, MatPrefix, MatSuffix} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {FormsModule} from '@angular/forms';
import {MatMenu, MatMenuItem, MatMenuTrigger} from '@angular/material/menu';
import {MatSelect} from '@angular/material/select';
import {MatOption} from '@angular/material/autocomplete';
import {MatDivider} from '@angular/material/list';
import {MatCheckbox} from '@angular/material/checkbox';
import {DurationSelectorComponent} from '../date';
import {ModifiersEditorComponent} from '../effect';
import {MatRadioButton, MatRadioGroup} from '@angular/material/radio';
import {MatExpansionPanelActionRow} from '@angular/material/expansion';
import {NhbkActionEditorDialogComponent, NhbkActionEditorDialogData} from '../action/nhbk-action-editor-dialog.component';
import {NhbkActionComponent} from '../action/nhbk-action.component';

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
