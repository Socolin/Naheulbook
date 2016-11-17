import {Component, OnInit} from '@angular/core';
import {Router, ActivatedRoute} from '@angular/router';
import {NotificationsService} from '../notifications';

import {ItemTemplate} from "./item-template.model";
import {ItemService} from "./item.service";

@Component({
    selector: 'edit-item',
    templateUrl: 'edit-item.component.html',
})
export class EditItemComponent implements OnInit {
    private item: ItemTemplate;
    private saving: boolean = false;
    private successMessage: string;

    constructor(private _router: Router
        , private _route: ActivatedRoute
        , private _notification: NotificationsService
        , private _itemService: ItemService) {
    }

    edit(showNext: boolean) {
        this.saving = true;
        this.successMessage = null;
        this._itemService.editItemTemplate(this.item).subscribe(
            item => {
                this.successMessage = "Objet bien sauvegarde: " + item.name;
                this.saving = false;
                setTimeout((function () {
                    if (showNext) {
                        this._router.navigate(['/edit-item', this.item.id + 1]);
                    } else {
                        this._router.navigate(['/database/items'], {queryParams: {id: 1}});
                    }
                }).bind(this), 300);
            },
            error => {
                this.saving = false;
            }
        );
    }

    ngOnInit() {
        this._route.params.subscribe(params => {
            let id = +params['id'];
            this._itemService.getItem(id).subscribe(
                item => {
                    this.item = item;
                }
            );

        });
    }
}
