import {Component, OnInit} from '@angular/core';
import {Router, ActivatedRoute} from '@angular/router';
import {NotificationsService} from '../notifications';

import {ItemEditorComponent} from "./item-editor.component";
import {ItemTemplate} from "./item-template.model";
import {ItemService} from "./item.service";

@Component({
    moduleId: module.id,
    selector: 'edit-item',
    templateUrl: 'edit-item.component.html',
    directives: [ItemEditorComponent]
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
                        this._router.navigate(['/database/items'], {queryParams: {id: this.item.category.type}});
                    }
                }).bind(this), 300);
            },
            error => {
                console.log(error);
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
                },
                error => {
                    console.log(error);
                    this._notification.error('Erreur', 'Erreur serveur');
                }
            );

        });
    }
}
