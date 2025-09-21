import {Component, Inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogClose } from '@angular/material/dialog';
import {Character} from '../character';
import { MatToolbar } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { CharacterComponent } from '../character/character.component';

export interface CharacterSheetDialogData {
    character: Character;
}

@Component({
    selector: 'app-character-sheet-dialog',
    templateUrl: './character-sheet-dialog.component.html',
    styleUrls: ['./character-sheet-dialog.component.scss', '../shared/full-screen-dialog.scss'],
    imports: [MatToolbar, MatIconButton, MatDialogClose, MatIcon, CharacterComponent]
})
export class CharacterSheetDialogComponent implements OnInit {

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: CharacterSheetDialogData
    ) {
    }

    ngOnInit() {
    }

}
