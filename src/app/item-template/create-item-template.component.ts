import {Component, OnInit} from '@angular/core';

import {NotificationsService} from '../notifications';
import {LoginService} from '../user';

import {ItemTemplate} from './item-template.model';
import {ItemTemplateService} from './item-template.service';
import {ActivatedRoute} from '@angular/router';

@Component({
    selector: 'create-item-template',
    templateUrl: './create-item-template.component.html'
})
export class CreateItemTemplateComponent implements OnInit {
    public item: ItemTemplate = new ItemTemplate();
    public lastItem: ItemTemplate;
    public saving = false;

    constructor(private _itemTemplateService: ItemTemplateService,
                private _notifications: NotificationsService,
                private _route: ActivatedRoute,
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

    ngOnInit(): void {
        if (this._route.snapshot.queryParams['copyFrom']) {
            this._itemTemplateService.getItem(+this._route.snapshot.queryParams['copyFrom']).subscribe(item => {
                this.item = item;
                this.item.source = 'private';
            });
        }
    }
}
