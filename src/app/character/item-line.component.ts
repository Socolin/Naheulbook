import {Component, Input, OnInit} from '@angular/core';
import {MatMenuPanel} from '@angular/material/menu';

import {Item} from '../item';
import {Character} from './character.model';
import {CharacterItemDialogComponent, CharacterItemDialogData} from './character-item-dialog.component';
import {MatDialog} from '@angular/material/dialog';
import {ItemActionService} from './item-action.service';

@Component({
    selector: 'app-item-line',
    templateUrl: './item-line.component.html',
    styleUrls: ['./item-line.component.scss']
})
export class ItemLineComponent {
    @Input() character?: Character;
    @Input() item: Item;
    @Input() itemMenu: MatMenuPanel;
    @Input() hideEquippedMarker: boolean;
    @Input() hideShouldPutIntoContainerMarker: boolean;
    @Input() gmView: boolean;

    constructor(
        private readonly dialog: MatDialog,
        private readonly itemActionService: ItemActionService
    ) {
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

}
