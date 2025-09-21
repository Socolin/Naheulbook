import {Component, Input} from '@angular/core';
import {MatMenuPanel} from '@angular/material/menu';

import {Item} from '../item';
import {Character} from './character.model';
import {CharacterItemDialogComponent, CharacterItemDialogData} from './character-item-dialog.component';
import {ItemActionService} from './item-action.service';
import {NhbkMatDialog} from '../material-workaround';

@Component({
    selector: 'app-item-line',
    templateUrl: './item-line.component.html',
    styleUrls: ['./item-line.component.scss'],
    standalone: false
})
export class ItemLineComponent {
    @Input() character?: Character;
    @Input() item: Item;
    @Input() itemMenu: MatMenuPanel;
    @Input() hideEquippedMarker: boolean;
    @Input() hideShouldPutIntoContainerMarker: boolean;
    @Input() gmView: boolean;
    @Input() itemInLoot: boolean;

    constructor(
        private readonly dialog: NhbkMatDialog,
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
                    itemActionService: this.itemActionService,
                    itemInLoot: this.itemInLoot
                }
            });
    }

}
