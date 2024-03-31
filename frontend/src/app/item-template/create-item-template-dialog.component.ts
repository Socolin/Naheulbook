import {Component, Inject, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications';
import {LoginService} from '../user';

import {ItemTemplate} from './item-template.model';
import {ItemTemplateService} from './item-template.service';
import { MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA, MatLegacyDialogRef as MatDialogRef } from '@angular/material/legacy-dialog';
import {Guid} from '../api/shared/util';

export interface CreateItemTemplateDialogData {
    copyFromItemTemplateId?: Guid;
}

@Component({
    templateUrl: './create-item-template-dialog.component.html',
    styleUrls: ['./create-item-template-dialog.component.scss', '../shared/full-screen-dialog.scss']
})
export class CreateItemTemplateDialogComponent implements OnInit {
    public item: ItemTemplate = new ItemTemplate();
    public saving = false;

    constructor(
        private readonly itemTemplateService: ItemTemplateService,
        private readonly notifications: NotificationsService,
        private readonly loginService: LoginService,
        private readonly dialogRef: MatDialogRef<CreateItemTemplateDialogComponent, ItemTemplate>,
        @Inject(MAT_DIALOG_DATA) private readonly data: CreateItemTemplateDialogData,
    ) {
        if (this.loginService.currentLoggedUser && this.loginService.currentLoggedUser.admin) {
            this.item.source = 'official';
        } else {
            this.item.source = 'community';
        }
    }

    canCreateItemTemplate() {
        return this.item.name && this.item.subCategoryId && this.item.source
    }

    createItemTemplate() {
        if (!this.canCreateItemTemplate()) {
            return;
        }

        this.saving = true;
        let {id: itemTemplateId, skillModifiers, skills, unSkills, ...baseRequest} = this.item;
        const request = {
            ...baseRequest,
            skillIds: skills.map(s => s.id),
            unSkillIds: skills.map(s => s.id),
            skillModifiers: skillModifiers && skillModifiers.map(s => ({value: s.value, skillId: s.skill.id}))
        };
        this.itemTemplateService.createItemTemplate(request).subscribe(
            itemTemplate => {
                this.saving = false;
                this.notifications.info('Objet', 'Objet créé: ' + itemTemplate.name);
                this.dialogRef.close(itemTemplate);
            }, () => {
                this.saving = false;
            }
        );
    }

    ngOnInit(): void {
        if (this.data && this.data.copyFromItemTemplateId) {
            this.itemTemplateService.getItem(this.data.copyFromItemTemplateId).subscribe(item => {
                this.item = item;
                this.item.source = 'private';
            });
        }
    }
}
