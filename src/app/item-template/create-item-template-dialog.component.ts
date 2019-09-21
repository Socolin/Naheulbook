import {Component, Inject, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications';
import {LoginService} from '../user';

import {ItemTemplate} from './item-template.model';
import {ItemTemplateService} from './item-template.service';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material';

export interface CreateItemTemplateDialogData {
    copyFromItemTemplateId?: number;
}

@Component({
    templateUrl: './create-item-template-dialog.component.html',
    styleUrls: ['./create-item-template-dialog.component.scss']
})
export class CreateItemTemplateDialogComponent implements OnInit {
    public item: ItemTemplate = new ItemTemplate();
    public saving = false;

    constructor(
        private _itemTemplateService: ItemTemplateService,
        private _notifications: NotificationsService,
        private _loginService: LoginService,
        private dialogRef: MatDialogRef<CreateItemTemplateDialogComponent, ItemTemplate>,
        @Inject(MAT_DIALOG_DATA) private data: CreateItemTemplateDialogData,
    ) {
        if (this._loginService.currentLoggedUser && this._loginService.currentLoggedUser.admin) {
            this.item.source = 'official';
        } else {
            this.item.source = 'community';
        }
    }

    canCreateItemTemplate() {
        return this.item.name && this.item.categoryId && this.item.source
    }

    createItemTemplate() {
        if (!this.canCreateItemTemplate()) {
            return;
        }

        this.saving = true;
        let {id: itemTemplateId, skillModifiers, ...baseRequest} = this.item;
        const request = {
            ...baseRequest,
            skillModifiers: skillModifiers.map(s => ({value: s.value, skill: s.skill.id}))
        };
        this._itemTemplateService.createItemTemplate(request).subscribe(
            itemTemplate => {
                this.saving = false;
                this._notifications.info('Objet', 'Objet créé: ' + itemTemplate.name);
                this.dialogRef.close(itemTemplate);
            }, () => {
                this.saving = false;
            }
        );
    }

    ngOnInit(): void {
        if (this.data && this.data.copyFromItemTemplateId) {
            this._itemTemplateService.getItem(this.data.copyFromItemTemplateId).subscribe(item => {
                this.item = item;
                this.item.source = 'private';
            });
        }
    }
}
