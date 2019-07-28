import {Component, EventEmitter, Input, Output} from '@angular/core';
import {MatDialog} from '@angular/material';

import {God} from '../shared';
import {CreateItemTemplateDialogComponent, CreateItemTemplateDialogData} from './create-item-template-dialog.component';
import {EditItemTemplateDialogComponent, EditItemTemplateDialogData} from './edit-item-template-dialog.component';

import {ItemTemplate} from './item-template.model';

@Component({
    selector: 'item-template',
    styleUrls: ['./item-template.component.scss'],
    templateUrl: './item-template.component.html',
})
export class ItemTemplateComponent {
    @Input() itemTemplate: ItemTemplate;
    @Input() editable: boolean;
    @Input() copyable: boolean;
    @Input() originsName: {[originId: number]: string};
    @Input() jobsName: {[jobId: number]: string};
    @Input() godsByTechName: {[techName: string]: God};
    @Input() actions: string[];
    @Output() onAction = new EventEmitter<{action: string, data: any}>();

    constructor(
        private dialog: MatDialog
    ) {
    }

    openEditItemTemplateDialog(itemTemplate: ItemTemplate) {
        this.dialog.open<EditItemTemplateDialogComponent, EditItemTemplateDialogData>(
            EditItemTemplateDialogComponent,
            {
                minWidth: '100vw',
                height: '100vh',
                data: {itemTemplateId: itemTemplate.id}
            }
        );
    }

    openCreateItemTemplateDialog(sourceItem: ItemTemplate) {
        this.dialog.open<CreateItemTemplateDialogComponent, CreateItemTemplateDialogData>(
            CreateItemTemplateDialogComponent,
            {
                minWidth: '100vw',
                height: '100vh',
                data: {copyFromItemTemplateId: sourceItem.id}
            }
        );
    }

    hasAction(actionName: string) {
        return this.actions && this.actions.indexOf(actionName) !== -1;
    }

    emitAction(actionName: string, data: any) {
        this.onAction.emit({action: actionName, data: data});
    }
}
