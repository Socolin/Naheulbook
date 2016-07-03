import {Component, EventEmitter} from '@angular/core';

import {Character} from "../character";
import {Item} from "../character/item.model";

@Component({
    selector: 'item-detail',
    templateUrl: 'app/item/item-detail.component.html',
    inputs: ['item', 'character', 'characterStats'],
    outputs: ['itemAction'],
})
export class ItemDetailComponent {
    public item: Item;
    public character: Character;
    public quantityModifier: string;
    public modifiers: any[];
    public characterStats: any[];

    private itemAction: EventEmitter<any> = new EventEmitter<any>();

    ngOnChanges() {
        this.modifiers = [];
        if (this.item && this.item.template.modifiers) {
            for (let i = 0; i < this.item.template.modifiers.length; i++) {
                let modifier = this.item.template.modifiers[i];
                if (modifier.job && modifier.job != this.character.job.id) {
                    continue;
                }
                if (modifier.origin && modifier.origin != this.character.origin.id) {
                    continue;
                }
                let newModifier = JSON.parse(JSON.stringify(modifier));
                for (var j = 0; j < this.modifiers.length; j++) {
                    let newMod = this.modifiers[j];
                    if (newModifier.stat == newMod.stat
                        && newModifier.type == newMod.type
                        && (!newModifier.special || newModifier.special.length == 0)
                        && (!newMod.special || newMod.special.length == 0)) {
                        newMod.value += newModifier.value;
                        newModifier = null;
                        break;
                    }
                }
                if (newModifier) {
                    this.modifiers.push(newModifier);
                }
            }
        }
    }

    updateQuantity() {
        if (isNaN(parseInt(this.quantityModifier))) {
            this.quantityModifier = null;
            return;
        }

        this.itemAction.emit({action: 'update_quantity', quantity: this.quantityModifier});
        this.quantityModifier = null;
        return false;
    }

    public itemEditName: string;
    public itemEditDescription: string;
    public editing: boolean;

    editItem() {
        this.editing = !this.editing;
        if (this.editing) {
            this.itemEditName = this.item.name;
            this.itemEditDescription = this.item.description;
        } else {
            if (this.itemEditName != this.item.name
                || this.itemEditDescription != this.item.description) {
                this.itemAction.emit({
                    action: 'edit_item_name',
                    name: this.itemEditName,
                    description: this.itemEditDescription
                })
            }
        }
    }
}
