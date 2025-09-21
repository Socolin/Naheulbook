import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {Item} from '../item';
import {Character, CharacterGiveDestination} from './character.model';
import {CharacterService} from './character.service';

export interface GiveItemDialogData {
    item: Item;
    character: Character;
}

export interface GiveItemDialogResult {
    giveTarget: CharacterGiveDestination;
    giveQuantity?: number;
}

@Component({
    templateUrl: './give-item-dialog.component.html',
    styleUrls: ['./give-item-dialog.component.scss'],
    standalone: false
})
export class GiveItemDialogComponent implements OnInit {
    public giveTarget?: CharacterGiveDestination;
    public giveDestination?: CharacterGiveDestination[];
    public giveQuantity?: number;

    constructor(
        private readonly characterService: CharacterService,
        private readonly dialogRef: MatDialogRef<GiveItemDialogComponent, GiveItemDialogResult>,
        @Inject(MAT_DIALOG_DATA) public readonly data: GiveItemDialogData,
    ) {
        this.giveQuantity = data.item.data.quantity
    }

    giveItem() {
        if (!this.giveTarget) {
            return;
        }

        this.dialogRef.close({
            giveQuantity: this.giveQuantity,
            giveTarget: this.giveTarget
        })
    }

    ngOnInit() {
        this.characterService.listActiveCharactersInGroup(this.data.character.group!.id).subscribe(
            characters => {
                this.giveDestination = characters.filter(c => c.id !== this.data.character.id);
            }
        );
    }
}
