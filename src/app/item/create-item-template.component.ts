import {Component} from '@angular/core';

import {NotificationsService} from '../notifications';
import {LoginService} from '../user';

import {
    ItemTemplate,
    ItemTemplateService
} from '.';

@Component({
    selector: 'create-item-template',
    templateUrl: './create-item-template.component.html'
})
export class CreateItemTemplateComponent {
    public item: ItemTemplate = new ItemTemplate();
    public lastItem: ItemTemplate;
    public saving = false;

    constructor(private _itemTemplateService: ItemTemplateService,
                private _notifications: NotificationsService,
                private _loginService: LoginService) {
        if (this._loginService.currentLoggedUser && this._loginService.currentLoggedUser.admin) {
            this.item.source = 'official';
        }
        else {
            this.item.source = 'community';
        }
    }

    create() {
        if (!this.item.category) {
            return false;
        }
        if (!this.item.name) {
            return false;
        }

        this.saving = true;
        this._itemTemplateService.create(this.item).subscribe(
            item => {
                this._notifications.info('Objet', 'Objet créé: ' + item.name);
                this.lastItem = item;
                this.item = new ItemTemplate();
                this.item.source = this.lastItem.source;
                this.item.data.price = this.lastItem.data.price;
                if (this.lastItem.data.diceDrop) {
                    this.item.data.diceDrop = this.lastItem.data.diceDrop + 1;
                }
            }
        );
    }
}
