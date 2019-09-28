import {forkJoin} from 'rxjs';
import {
    Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild
} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {
    ActiveStatsModifier,
    God,
    IconSelectorComponent,
    IconSelectorComponentDialogData,
    MiscService,
    NhbkDialogService
} from '../shared';

import {ItemTemplateService, ItemTemplateCategory} from '../item-template';

import {Character, CharacterGiveDestination} from './character.model';
import {CharacterService} from './character.service';
import {Item} from '../item';
import {ItemActionService} from './item-action.service';
import {MatDialog} from '@angular/material';
import {IconDescription} from '../shared/icon.model';
import {EditItemDialogComponent, EditItemDialogData, EditItemDialogResult} from './edit-item-dialog.component';
import {AddItemModifierDialogComponent} from './add-item-modifier-dialog.component';
import {IDurable} from '../api/shared';

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

    public itemCategoriesById: {[categoryId: number]: ItemTemplateCategory};
    public modifiers: any[];

    @ViewChild('giveItemDialog', {static: true})
    public giveItemDialog: Portal<any>;
    public giveItemOverlayRef: OverlayRef | undefined;
    public giveDestination: CharacterGiveDestination[] | undefined;
    public giveTarget: CharacterGiveDestination;
    public giveQuantity: number | undefined;

    @ViewChild('storeItemInContainerDialog', {static: true})
    public storeItemInContainerDialog: Portal<any>;
    public storeItemInContainerOverlayRef: OverlayRef | undefined;

    @ViewChild('lifetimeDialog', {static: true})
    public lifetimeDialog: Portal<any>;
    public lifetimeOverlayRef: OverlayRef | undefined;
    public previousLifetime: IDurable | null;

    constructor(
        public readonly itemActionService: ItemActionService,
        private readonly itemTemplateService: ItemTemplateService,
        private readonly characterService: CharacterService,
        private readonly miscService: MiscService,
        private readonly nhbkDialogService: NhbkDialogService,
        private readonly dialog: MatDialog,
    ) {
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
                if (modifier.job && !this.character.hasJob(modifier.job)) {
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
        const dialogRef = this.dialog.open<EditItemDialogComponent, EditItemDialogData, EditItemDialogResult>(EditItemDialogComponent, {
            data: {item: this.item}
        });
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }
            this.itemActionService.onAction('edit_item_name', this.item, {
                name: result.name,
                description: result.description
            });
        });
    }

    /*
     * Give item dialog
     */

    openGiveItemDialog(item: Item) {
        if (!this.character.group) {
            return;
        }
        this.giveQuantity = item.data.quantity;
        this.giveItemOverlayRef = this.nhbkDialogService.openCenteredBackdropDialog(this.giveItemDialog);
        this.giveDestination = undefined;

        this.characterService.listActiveCharactersInGroup(this.character.group.id).subscribe(
            characters => {
                this.giveDestination = characters.filter(c => c.id !== this.character.id);
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
        this.itemActionService.onAction('give', this.item, {characterId: this.giveTarget.id, quantity: this.giveQuantity});
        this.closeGiveItemDialog();
    }

    /*
     * Add modifier dialog
     */

    openModifierDialog() {
        const dialogRef = this.dialog.open<AddItemModifierDialogComponent, any, ActiveStatsModifier>(AddItemModifierDialogComponent, {
            minWidth: '100vw',
            height: '100vh'
        });
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }

            if (this.item.modifiers === null) {
                this.item.modifiers = [];
            }
            this.activeModifier(result);
            this.item.modifiers.push(result);
            this.itemActionService.onAction('update_modifiers', this.item);
        });
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

    /*
     * Modifier
     */

    removeModifier(index: number) {
        if (this.item.modifiers) {
            this.item.modifiers.splice(index, 1);
            this.itemActionService.onAction('update_modifiers', this.item);
        }
    }

    updateModifier(index: number) {
        if (this.item.modifiers) {
            if (this.item.modifiers[index].active) {
                this.activeModifier(this.item.modifiers[index]);
            }
            this.itemActionService.onAction('update_modifiers', this.item);
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

        this.lifetimeOverlayRef = this.nhbkDialogService.openCenteredBackdropDialog(this.lifetimeDialog);
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
        this.itemActionService.onAction('update_data', this.item);
        this.closeLifetimeDialog();
    }


    openStoreItemInContainerDialog() {
        this.storeItemInContainerOverlayRef = this.nhbkDialogService.openCenteredBackdropDialog(this.storeItemInContainerDialog);
    }

    closeStoreItemInContainerDialog() {
        if (!this.storeItemInContainerOverlayRef) {
            return;
        }
        this.storeItemInContainerOverlayRef.detach();
        this.storeItemInContainerOverlayRef = undefined;
    }

    removeItemFromContainer() {
        this.itemActionService.onAction('move_to_container', this.item, {container: null});
        this.closeStoreItemInContainerDialog();
    }

    isActionVisible(actionName: string) {
        return this.hiddenActions.indexOf(actionName) === -1;
    }

    nothing() {
        // This function allow to propagate swipeleft/right event to top, blocked due to overflow auto
    }

    ngOnInit() {
        forkJoin([
            this.itemTemplateService.getCategoriesById(),
            this.miscService.getGodsByTechName(),
        ]).subscribe(
            ([categoriesById, godsByTechName]) => {
                this.godsByTechName = godsByTechName;
                this.itemCategoriesById = categoriesById;
            }
        );
    }

    openSelectIconDialog() {
        if (this.readonly) {
            return;
        }

        const dialogRef = this.dialog.open<IconSelectorComponent, IconSelectorComponentDialogData, IconDescription>(IconSelectorComponent, {
            data: {icon: this.item.data.icon}
        });

        dialogRef.afterClosed().subscribe((icon) => {
            if (!icon) {
                return;
            }
            this.itemActionService.onAction('change_icon', this.item, {icon: icon});
        })
    }
}
