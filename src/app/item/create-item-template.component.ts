import {Component} from '@angular/core';

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
    public errorMessage: string;
    public successMessage: string;

    constructor(private _itemTemplateService: ItemTemplateService) {
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
        this._itemTemplateService.create(this.item).subscribe(
            item => {
                this.successMessage = 'Objet créé: ' + item.name;
                this.lastItem = item;
                this.item = new ItemTemplate();
                this.item.data.price = this.lastItem.data.price;
                if (this.lastItem.data.diceDrop) {
                    this.item.data.diceDrop = this.lastItem.data.diceDrop + 1;
                }
            }
        );
    }
}
