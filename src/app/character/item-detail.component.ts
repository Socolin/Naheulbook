import {
    Component, Input, OnChanges, OnInit, Optional, SimpleChanges, ViewChild
} from '@angular/core';
import {Portal, OverlayRef} from '@angular/material';

import {ActiveStatsModifier, God, MiscService, NhbkDialogService} from '../shared';

import {IDurable} from '../date/durable.model';
import {ItemTemplateService, ItemCategory} from '../item';

import {Character, CharacterGiveDestination} from './character.model';
import {CharacterService} from './character.service';
import {Item} from './item.model';
import {ItemActionService} from './item-action.service';
import {Observable} from 'rxjs/Observable';

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
    @Input() jobsName?: {[id: number]: string};
    @Input() originsName?: {[id: number]: string};
    @Input() godsByTechName: {[techName: string]: God};
    @Input() hiddenActions: string[] = [];

    public itemCategoriesById: {[categoryId: number]: ItemCategory};
    public modifiers: any[];

    public itemEditName: string;
    public itemEditDescription: string | undefined;
    public editing: boolean;

    @ViewChild('giveItemDialog')
    public giveItemDialog: Portal<any>;
    public giveItemOverlayRef: OverlayRef | undefined;
    public giveDestination: CharacterGiveDestination[] | undefined;
    public giveTarget: CharacterGiveDestination;
    public giveQuantity: number | undefined;

    @ViewChild('addModifierDialog')
    public addModifierDialog: Portal<any>;
    public addModifierOverlayRef: OverlayRef | undefined;
    public newItemModifier: ActiveStatsModifier;

    @ViewChild('storeItemInContainerDialog')
    public storeItemInContainerDialog: Portal<any>;
    public storeItemInContainerOverlayRef: OverlayRef | undefined;

    @ViewChild('lifetimeDialog')
    public lifetimeDialog: Portal<any>;
    public lifetimeOverlayRef: OverlayRef | undefined;
    public previousLifetime: IDurable | null;

    constructor(private _itemTemplateService: ItemTemplateService
        , public _itemActionService: ItemActionService
        , private _characterService: CharacterService
        , private _miscService: MiscService
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
                if (modifier.job && (!this.character.job || modifier.job !== this.character.job.id)) {
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

    openGiveItemDialog(item: Item) {
        this.giveQuantity = item.data.quantity;
        this.giveItemOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.giveItemDialog);

        this._characterService.listActiveCharactersInGroup(this.character.id).subscribe(
            characters => {
                this.giveDestination = characters;
            }
        );
    }

    closeGiveItemDialog() {
        if (!this.giveItemOverlayRef) {
            return;
        }
        this.giveItemOverlayRef.detach();
        this.giveItemOverlayRef = undefined;
    }

    giveItem() {
        this._itemActionService.onAction('give', this.item, {characterId: this.giveTarget.id, quantity: this.giveQuantity});
        this.closeGiveItemDialog();
    }

    /*
     * Add modifier dialog
     */

    openModifierDialog() {
        this.newItemModifier = new ActiveStatsModifier();
        this.addModifierOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.addModifierDialog);
    }

    closeModifierDialog() {
        if (!this.addModifierOverlayRef) {
            return;
        }
        this.addModifierOverlayRef.detach();
        this.addModifierOverlayRef = undefined;
    }

    activeModifier(modifier: ActiveStatsModifier) {
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
        if (!this.lifetimeOverlayRef) {
            return;
        }
        this.lifetimeOverlayRef.detach();
        this.lifetimeOverlayRef = undefined;
    }

    cancelLifetimeDialog() {
        this.item.data.lifetime = this.previousLifetime;
        this.closeLifetimeDialog();
    }

    updateLifetime() {
        if (this.item.data.lifetime && this.item.data.lifetime.durationType === 'forever') {
            this.item.data.lifetime = null;
        }
        this._itemActionService.onAction('update_data', this.item);
        this.closeLifetimeDialog();
    }


    openStoreItemInContainerDialog() {
        this.storeItemInContainerOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.storeItemInContainerDialog);
    }

    closeStoreItemInContainerDialog() {
        if (!this.storeItemInContainerOverlayRef) {
            return;
        }
        this.storeItemInContainerOverlayRef.detach();
        this.storeItemInContainerOverlayRef = undefined;
    }

    removeItemFromContainer() {
        this._itemActionService.onAction('move_to_container', this.item, {container: null})
        this.closeStoreItemInContainerDialog();
    }

    isActionVisible(actionName: string) {
        return this.hiddenActions.indexOf(actionName) === -1;
    }

    nothing() {
        // This function allow to propagate swipeleft/right event to top, blocked due to overflow auto
    }

    ngOnInit() {
        Observable.forkJoin(
            this._itemTemplateService.getCategoriesById(),
            this._miscService.getGodsByTechName(),
        ).subscribe(
            ([categoriesById, godsByTechName]) => {
                this.godsByTechName = godsByTechName;
                this.itemCategoriesById = categoriesById;
            }
        );
    }
}
