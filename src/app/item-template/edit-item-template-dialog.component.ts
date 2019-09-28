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
        private readonly dialogRef: MatDialogRef<EditItemTemplateDialogComponent, ItemTemplate>,
        @Inject(MAT_DIALOG_DATA) private readonly data: EditItemTemplateDialogData,
        private readonly itemTemplateService: ItemTemplateService,
        private readonly notification: NotificationsService,
    ) {
    }

    saveChanges() {
        this.saving = true;
        let {id: itemTemplateId, skillModifiers, ...baseRequest} = this.item;
        const request = {
            ...baseRequest,
            skillModifiers: skillModifiers.map(s => ({value: s.value, skill: s.skill.id}))
        };
        this.itemTemplateService.editItemTemplate(itemTemplateId, request).subscribe(
            itemTemplate => {
                this.notification.success('Objet', 'Objet sauvegardé: ' + itemTemplate.name);
                this.saving = false;
                this.dialogRef.close(itemTemplate);
            },
            (err) => {
                this.saving = false;
                throw err;
            }
        );
    }

    ngOnInit() {
        this.itemTemplateService.getItem(this.data.itemTemplateId).subscribe(
            itemTemplate => {
                this.item = itemTemplate;
            }
        );
    }
}
