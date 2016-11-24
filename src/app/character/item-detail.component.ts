import {Component, Input, OnChanges, OnInit, forwardRef, Inject, SimpleChanges} from '@angular/core';
import {Item, ItemModifier} from './item.model';
import {Character, CharacterGiveDestination} from './character.model';
import {ItemService, ItemCategory} from '../item';
import {ItemActionService} from './item-action.service';
import {GroupService} from '../group/group.service';

@Component({
    selector: 'item-detail',
    templateUrl: 'item-detail.component.html'
})
export class ItemDetailComponent implements OnChanges, OnInit {
    @Input() item: Item;
    @Input() character: Character;
    @Input() gmView: boolean;
    @Input() readonly: boolean;

    public itemCategoriesById: {[categoryId: number]: ItemCategory};
    public modifiers: any[];

    public itemEditName: string;
    public itemEditDescription: string;
    public editing: boolean;

    public givingItem: boolean = false;
    public giveDestination: CharacterGiveDestination[];

    public addingModifier: boolean = false;
    public newModifier: ItemModifier;

    constructor(@Inject(forwardRef(() => ItemService)) private _itemService: ItemService
        , private _itemActionService: ItemActionService
        , private _groupService: GroupService) {
    }

    ngOnChanges(changes: SimpleChanges) {
        if ('item' in changes
            && changes['item'].previousValue
            && changes['item'].currentValue
            && changes['item'].currentValue['id'] != changes['item'].previousValue['id']) {
            this.givingItem = false;
        }
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
                this._itemActionService.onAction('edit_item_name', this.item, {
                    name: this.itemEditName,
                    description: this.itemEditDescription
                });
            }
        }
    }

    startGiveItem() {
        this.givingItem = true;
        this._groupService.listActiveCharactersInGroup(this.character.id).subscribe(
            characters => {
                this.giveDestination = characters;
            }
        );
    }

    startAddModifier() {
        this.newModifier = new ItemModifier();
        this.addingModifier = true;
    }

    stopAddModifier() {
        this.addingModifier = false;
    }

    addModifier() {
        this.addingModifier = false;
        if (this.item.modifiers === null) {
            this.item.modifiers = [];
        }
        this.newModifier.active = true;
        this.newModifier.currentDuration = this.newModifier.duration;
        this.item.modifiers.push(this.newModifier);
        this._itemActionService.onAction('update_modifiers', this.item);
        this.newModifier = new ItemModifier();
    }

    removeModifier(index: number) {
        if (this.item.modifiers) {
            this.item.modifiers.splice(index, 1);
            if (this.item.modifiers.length == 0) {
                this.item.modifiers = null;
            }
            this._itemActionService.onAction('update_modifiers', this.item);
        }
    }

    toggleModifier(index: number) {
        if (this.item.modifiers) {
            this.item.modifiers[index].active = !this.item.modifiers[index].active;
            if (this.item.modifiers[index].active) {
                this.item.modifiers[index].currentDuration = this.item.modifiers[index].duration;
            }
            this._itemActionService.onAction('update_modifiers', this.item);
        }
    }

    ngOnInit() {
        this._itemService.getCategoriesById().subscribe(
            categoriesById => {
                this.itemCategoriesById = categoriesById;
            },
        );
    }
}
