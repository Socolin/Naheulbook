import {Component, EventEmitter, Input, Output, OnChanges, OnInit} from '@angular/core';
import {Item} from './item.model';
import {Character} from './character.model';
import {ValueEditorComponent} from '../shared';
import {ItemService} from '../item/item.service';
import {ItemCategory} from '../item/item-template.model';

@Component({
    moduleId: module.id,
    directives: [ValueEditorComponent],
    selector: 'item-detail',
    templateUrl: 'item-detail.component.html'
})
export class ItemDetailComponent implements OnChanges, OnInit {
    @Input() item: Item;
    @Input() character: Character;
    @Input() characterStats: any[];

    @Output() itemAction: EventEmitter<any> = new EventEmitter<any>();

    public itemCategoriesById: {[categoryId: number]: ItemCategory};
    public quantityModifier: string;
    public modifiers: any[];

    public itemEditName: string;
    public itemEditDescription: string;
    public editing: boolean;

    constructor(private _itemService: ItemService) {
    }

    ngOnChanges() {
        this.modifiers = [];
        if (this.item && this.item.template.modifiers) {
            for (let i = 0; i < this.item.template.modifiers.length; i++) {
                let modifier = this.item.template.modifiers[i];
                if (modifier.job && modifier.job !== this.character.job.id) {
                    continue;
                }
                if (modifier.origin && modifier.origin !== this.character.origin.id) {
                    continue;
                }
                let newModifier = JSON.parse(JSON.stringify(modifier));
                for (let j = 0; j < this.modifiers.length; j++) {
                    let newMod = this.modifiers[j];
                    if (newModifier.stat === newMod.stat
                        && newModifier.type === newMod.type
                        && (!newModifier.special || newModifier.special.length === 0)
                        && (!newMod.special || newMod.special.length === 0)) {
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

    editItem() {
        this.editing = !this.editing;
        if (this.editing) {
            this.itemEditName = this.item.data.name;
            this.itemEditDescription = this.item.data.description;
        } else {
            if (this.itemEditName !== this.item.data.name
                || this.itemEditDescription !== this.item.data.description) {
                this.itemAction.emit({
                    action: 'edit_item_name',
                    name: this.itemEditName,
                    description: this.itemEditDescription
                });
            }
        }
    }

    ngOnInit() {

        this._itemService.getCategoriesById().subscribe(
            categoriesById => {
                this.itemCategoriesById = categoriesById;
            },
            err => {
                console.log(err);
            }
        );
    }
}
