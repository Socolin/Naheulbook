import {Component, Input, OnInit} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';

import {Item} from '../item';
import {Character} from './character.model';
import {EditItemDialogComponent, EditItemDialogData, EditItemDialogResult} from './edit-item-dialog.component';
import {AddItemModifierDialogComponent} from './add-item-modifier-dialog.component';
import {ActiveStatsModifier} from '../shared';
import {ItemActionService} from './item-action.service';
import {CharacterItemDialogComponent, CharacterItemDialogData} from './character-item-dialog.component';

@Component({
    selector: 'app-item-line',
    templateUrl: './item-line.component.html',
    styleUrls: ['./item-line.component.scss']
})
export class ItemLineComponent implements OnInit {
    @Input() character: Character;
    @Input() item: Item;
    @Input() hideEquippedMarker: boolean;
    @Input() gmView: boolean;

    constructor(
        private readonly dialog: MatDialog,
        private readonly itemActionService: ItemActionService
    ) {
    }

    editItem(item: Item) {
        const dialogRef = this.dialog.open<EditItemDialogComponent, EditItemDialogData, EditItemDialogResult>(
            EditItemDialogComponent,
            {
                data: {item: item}
            }
        );
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }
            this.itemActionService.onAction('edit_item_name', item, {
                name: result.name,
                description: result.description
            });
        });
    }

    openModifierDialog(item: Item) {
        const dialogRef = this.dialog.open<AddItemModifierDialogComponent, any, ActiveStatsModifier>(AddItemModifierDialogComponent, {
            minWidth: '100vw',
            height: '100vh'
        });
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }

            if (item.modifiers === null) {
                item.modifiers = [];
            }
            this.activeModifier(result);
            item.modifiers.push(result);
            this.itemActionService.onAction('update_modifiers', item);
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

    openCharacterItemDialog(item: Item) {
        this.dialog.open<CharacterItemDialogComponent, CharacterItemDialogData>(
            CharacterItemDialogComponent,
            {
                autoFocus: false,
                data: {
                    item,
                    character: this.character,
                    gmView: this.gmView,
                    itemActionService: this.itemActionService
                }
            });
    }

    ngOnInit() {
    }
}
