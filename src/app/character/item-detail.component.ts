import {
    Component, Input, OnChanges, OnInit, forwardRef, Inject, SimpleChanges, ViewChild,
} from '@angular/core';
import {Portal, OverlayRef} from '@angular/material';

import {NhbkDialogService} from '../shared/nhbk-dialog.service';
import {dateOffset2TimeDuration, timeDuration2DateOffset2} from '../date/util';
import {ItemService, ItemCategory} from '../item';
import {GroupService} from '../group/group.service';
import {NhbkDateOffset} from '../date/date.model';
import {Item, ItemModifier} from './item.model';
import {Character, CharacterGiveDestination} from './character.model';
import {ItemActionService} from './item-action.service';

@Component({
    selector: 'item-detail',
    templateUrl: './item-detail.component.html',
    styleUrls: ['./item-detail.component.scss'],
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

    @ViewChild('giveItemDialog')
    public giveItemDialog: Portal<any>;
    public giveItemOverlayRef: OverlayRef;
    public giveDestination: CharacterGiveDestination[] = null;
    public giveTarget: CharacterGiveDestination;

    @ViewChild('addModifierDialog')
    public addModifierDialog: Portal<any>;
    public addModifierOverlayRef: OverlayRef;
    public newModifier: ItemModifier;

    @ViewChild('lifetimeDialog')
    public lifetimeDialog: Portal<any>;
    public lifetimeOverlayRef: OverlayRef;
    public previousLifetime: any;
    public lifetimeDateOffset: NhbkDateOffset = new NhbkDateOffset();

    constructor(@Inject(forwardRef(() => ItemService)) private _itemService: ItemService
        , public _itemActionService: ItemActionService
        , private _groupService: GroupService
        , private _nhbkDialogService: NhbkDialogService) {
    }

    ngOnChanges(changes: SimpleChanges) {
        if ('item' in changes
            && changes['item'].previousValue
            && changes['item'].currentValue
            && changes['item'].currentValue['id'] !== changes['item'].previousValue['id']) {
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

    /*
     * Give item dialog
     */

    openGiveItemDialog() {
        this.giveItemOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.giveItemDialog);

        this._groupService.listActiveCharactersInGroup(this.character.id).subscribe(
            characters => {
                this.giveDestination = characters;
            }
        );
    }

    closeGiveItemDialog() {
        this.giveItemOverlayRef.detach();
    }

    giveItem() {
        this._itemActionService.onAction('give', this.item, {characterId: this.giveTarget.id});
        this.closeGiveItemDialog();
    }

    /*
     * Add modifier dialog
     */

    openModifierDialog() {
        this.newModifier = new ItemModifier();

        this.addModifierOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.addModifierDialog);
    }

    closeModifierDialog() {
        this.addModifierOverlayRef.detach();
    }

    addModifier() {
        if (this.item.modifiers === null) {
            this.item.modifiers = [];
        }
        this.newModifier.active = true;
        this.newModifier.currentDuration = this.newModifier.duration;
        this.item.modifiers.push(this.newModifier);
        this._itemActionService.onAction('update_modifiers', this.item);
        this.newModifier = new ItemModifier();
        this.closeModifierDialog();
    }

    /*
     * Modifier
     */

    removeModifier(index: number) {
        if (this.item.modifiers) {
            this.item.modifiers.splice(index, 1);
            if (this.item.modifiers.length === 0) {
                this.item.modifiers = null;
            }
            this._itemActionService.onAction('update_modifiers', this.item);
        }
    }

    updateModifier(index: number) {
        if (this.item.modifiers) {
            if (this.item.modifiers[index].active) {
                this.item.modifiers[index].currentDuration = this.item.modifiers[index].duration;
            }
            this._itemActionService.onAction('update_modifiers', this.item);
        }
    }

    /*
     * Lifetime dialog
     */

    openLifetimeDialog() {
        this.previousLifetime = {type: this.item.data.lifetimeType, value: this.item.data.lifetime};
        if (this.item.data.lifetimeType === 'time') {
            if (!this.item.data.lifetime) {
                this.item.data.lifetime = 0;
            }
            this.lifetimeDateOffset = timeDuration2DateOffset2(+this.item.data.lifetime);
        }

        this.lifetimeOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.lifetimeDialog);
    }


    closeLifetimeDialog() {
        this.lifetimeOverlayRef.detach();
    }

    cancelLifetimeDialog() {
        this.item.data.lifetimeType = this.previousLifetime.type;
        this.item.data.lifetime = this.previousLifetime.value;
        this.closeLifetimeDialog();
    }

    updateLifetime() {
        this._itemActionService.onAction('update_data', this.item);
        this.closeLifetimeDialog();
    }

    setItemLifetimeDateOffset(dateOffset: NhbkDateOffset) {
        this.item.data.lifetime = dateOffset2TimeDuration(dateOffset);
    }

    updateLifetimeType() {
        let type = this.item.data.lifetimeType;
        if (type === null) {
            this.item.data.lifetime = null;
        } else {
            if (type === 'combat' || type === 'lap') {
                this.item.data.lifetime = 1;
            }
            else if (type === 'time') {
                if (this.lifetimeDateOffset) {
                    this.setItemLifetimeDateOffset(this.lifetimeDateOffset);
                }
                else {
                    this.item.data.lifetime = 0;
                }
            }
            else if (type === 'custom') {
                this.item.data.lifetime = '';
            }
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
