import {
    Component, Input, OnChanges, OnInit, forwardRef, Inject, SimpleChanges, ViewChild,
} from '@angular/core';
import {Portal, OverlayRef} from '@angular/material';

import {NhbkDialogService} from '../shared/nhbk-dialog.service';
import {ItemService, ItemCategory} from '../item';
import {GroupService} from '../group/group.service';
import {Item, ItemModifier} from './item.model';
import {Character, CharacterGiveDestination} from './character.model';
import {ItemActionService} from './item-action.service';
import {IDurable} from '../date/durable.model';

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
    public newItemModifier: ItemModifier;

    @ViewChild('lifetimeDialog')
    public lifetimeDialog: Portal<any>;
    public lifetimeOverlayRef: OverlayRef;
    public previousLifetime: IDurable;

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
        this.newItemModifier = new ItemModifier();
        this.addModifierOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.addModifierDialog);
    }

    closeModifierDialog() {
        this.addModifierOverlayRef.detach();
    }

    activeModifier(modifier: ItemModifier) {
        modifier.active = true;
        switch (modifier.durationType) {
            case 'time':
                modifier.currentTimeDuration = modifier.timeDuration;
                break;
            case 'combat':
                modifier.currentCombatCount = modifier.combatCount;
                break;
            case 'lap':
                modifier.currentLapCount = modifier.lapCount;
                break;
        }
    }

    addModifier() {
        if (this.item.modifiers === null) {
            this.item.modifiers = [];
        }
        this.activeModifier(this.newItemModifier);
        this.item.modifiers.push(this.newItemModifier);
        this._itemActionService.onAction('update_modifiers', this.item);
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
                this.activeModifier(this.item.modifiers[index]);
            }
            this._itemActionService.onAction('update_modifiers', this.item);
        }
    }

    /*
     * Lifetime dialog
     */

    openLifetimeDialog() {
        if (!this.item.data.lifetime) {
            this.item.data.lifetime = {durationType: 'forever'};
            this.previousLifetime = null;
        }
        else {
            this.previousLifetime = Object.assign({}, this.item.data.lifetime);
        }

        this.lifetimeOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.lifetimeDialog);
    }


    closeLifetimeDialog() {
        this.lifetimeOverlayRef.detach();
    }

    cancelLifetimeDialog() {
        this.item.data.lifetime = this.previousLifetime;
        this.closeLifetimeDialog();
    }

    updateLifetime() {
        if (this.item.data.lifetime.durationType === 'forever') {
            this.item.data.lifetime = null;
        }
        this._itemActionService.onAction('update_data', this.item);
        this.closeLifetimeDialog();
    }

    ngOnInit() {
        this._itemService.getCategoriesById().subscribe(
            categoriesById => {
                this.itemCategoriesById = categoriesById;
            },
        );
    }
}
