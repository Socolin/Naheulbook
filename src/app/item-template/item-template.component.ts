import {Component, EventEmitter, Input, Output} from '@angular/core';

import {God} from '../shared';
import {CreateItemTemplateDialogComponent, CreateItemTemplateDialogData} from './create-item-template-dialog.component';
import {EditItemTemplateDialogComponent, EditItemTemplateDialogData} from './edit-item-template-dialog.component';

import {ItemTemplate} from './item-template.model';
import {NhbkMatDialog} from '../material-workaround';

@Component({
    selector: 'item-template',
    styleUrls: ['./item-template.component.scss'],
    templateUrl: './item-template.component.html',
})
export class ItemTemplateComponent {
    @Input() itemTemplate: ItemTemplate;
    @Input() editable: boolean;
    @Input() copyable: boolean;
    @Input() originsName: {[originId: string]: string};
    @Input() jobsName: {[jobId: string]: string};
    @Input() godsByTechName: {[techName: string]: God};
    @Input() actions: string[];
    @Output() onAction = new EventEmitter<{action: string, data: any}>();
    @Output() onEdit = new EventEmitter<ItemTemplate>();
    @Output() onCreateCopy = new EventEmitter<ItemTemplate>();

    constructor(
        private readonly dialog: NhbkMatDialog,
    ) {
    }

    openEditItemTemplateDialog(itemTemplate: ItemTemplate) {
        const dialogRef = this.dialog.openFullScreen<EditItemTemplateDialogComponent, EditItemTemplateDialogData, ItemTemplate>(
            EditItemTemplateDialogComponent,
            {
                data: {itemTemplateId: itemTemplate.id}
            }
        );
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.onEdit.next(result);
        });
    }

    openCreateItemTemplateDialog(sourceItem: ItemTemplate) {
        const dialogRef = this.dialog.openFullScreen<CreateItemTemplateDialogComponent, CreateItemTemplateDialogData, ItemTemplate>(
            CreateItemTemplateDialogComponent,
            {
                data: {copyFromItemTemplateId: sourceItem.id}
            }
        );
        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.onEdit.next(result);
        });
    }

    hasAction(actionName: string) {
        return this.actions && this.actions.indexOf(actionName) !== -1;
    }

    emitAction(actionName: string, data: any) {
        this.onAction.emit({action: actionName, data: data});
    }
}
