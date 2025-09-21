import {Component, Inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import {Item} from '../item';
import {Character, CharacterGiveDestination} from './character.model';
import {CharacterService} from './character.service';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { IconComponent } from '../shared/icon.component';
import { ValueEditorComponent } from '../shared/value-editor.component';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatRadioGroup, MatRadioButton } from '@angular/material/radio';
import { FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';

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
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, IconComponent, ValueEditorComponent, MatProgressSpinner, MatRadioGroup, FormsModule, MatRadioButton, MatDialogActions, MatButton, MatDialogClose]
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
