import {Component, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {God, NhbkDialogService, removeDiacritics} from '../shared';
import {Skill} from '../skill';
import {NhbkAction, NhbkActionEditorDialogComponent, NhbkActionEditorDialogData} from '../action';
import {LoginService} from '../user';

import {ItemSlot, ItemTemplate} from './item-template.model';
import {ItemTemplateService} from './item-template.service';
import {itemTemplateModulesDefinitions} from './item-template-modules-definitions';
import {ItemTypeResponse} from '../api/responses';
import {NhbkMatDialog} from '../material-workaround';

@Component({
    selector: 'item-template-editor-module',
    styleUrls: ['./item-template-editor-module.component.scss'],
    templateUrl: './item-template-editor-module.component.html'
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
