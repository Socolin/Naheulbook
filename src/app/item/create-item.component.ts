import {Component} from '@angular/core';

import {ItemService} from "./item.service";
import {ItemEditorComponent} from "./item-editor.component";
import {ItemTemplate} from "./item-template.model";

@Component({
    moduleId: module.id,
    selector: 'create-item',
    templateUrl: 'create-item.component.html',
    providers: [ItemService],
    directives: [ItemEditorComponent]
})
export class CreateItemComponent {
    public item: ItemTemplate = new ItemTemplate();
    public lastItem: ItemTemplate;
    public saving: boolean = false;
    public errorMessage: string;
    public successMessage: string;

    constructor(private _itemService: ItemService) {
    }

    create() {
        if (!this.item.category) {
            return false;
        }
        if (!this.item.name) {
            return false;
        }

        this.saving = true;
        this.errorMessage = null;
        this.successMessage = null;
        this._itemService.create(this.item).subscribe(
            item => {
                this.successMessage = "Objet créé: " + item.name;
                this.lastItem = item;
                this.item = new ItemTemplate();
                this.item.data.price = this.lastItem.data.price;
                if (this.lastItem.data.diceDrop) {
                    this.item.data.diceDrop = this.lastItem.data.diceDrop + 1;
                }
            },
            error => {
                console.log(error);
                this.errorMessage = "Erreur lors de la sauvegarde de l'objet";
                this.saving = false;
            }
        );
    }
}
