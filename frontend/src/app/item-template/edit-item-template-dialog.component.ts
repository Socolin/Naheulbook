import {Component, OnInit, Inject} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

import {NotificationsService} from '../notifications';

import {ItemTemplate} from './item-template.model';
import {ItemTemplateService} from './item-template.service';
import {Guid} from '../api/shared/util';

export interface EditItemTemplateDialogData {
    itemTemplateId: Guid;
}

@Component({
    templateUrl: './edit-item-template-dialog.component.html',
    styleUrls: ['./edit-item-template-dialog.component.scss', '../shared/full-screen-dialog.scss'],
    standalone: false
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
        let {id: itemTemplateId, skillModifiers, skills, unSkills, ...baseRequest} = this.item;
        const request = {
            ...baseRequest,
            skillIds: skills.map(s => s.id),
            unSkillIds: unSkills.map(s => s.id),
            skillModifiers: skillModifiers.map(s => ({value: s.value, skillId: s.skill.id}))
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
