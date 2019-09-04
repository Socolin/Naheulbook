import {Component, OnInit, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material';

import {NotificationsService} from '../notifications';

import {ItemTemplate} from './item-template.model';
import {ItemTemplateService} from './item-template.service';

export interface EditItemTemplateDialogData {
    itemTemplateId: number;
}

@Component({
    templateUrl: './edit-item-template-dialog.component.html',
    styleUrls: ['./edit-item-template-dialog.component.scss', '../shared/full-screen-dialog.scss'],
})
export class EditItemTemplateDialogComponent implements OnInit {
    public item: ItemTemplate;
    public saving = false;

    constructor(
        private _notification: NotificationsService
        , private _itemTemplateService: ItemTemplateService
        , private dialogRef: MatDialogRef<EditItemTemplateDialogComponent>
        , @Inject(MAT_DIALOG_DATA) private data: EditItemTemplateDialogData
    ) {
    }

    saveChanges() {
        this.saving = true;
        this._itemTemplateService.editItemTemplate(this.item).subscribe(
            item => {
                this._notification.success('Objet', 'Objet sauvegardé: ' + item.name);
                this.saving = false;
                this.dialogRef.close();
            }
        );
    }

    ngOnInit() {
        this._itemTemplateService.getItem(this.data.itemTemplateId).subscribe(
            item => {
                this.item = item;
            }
        );
    }
}
