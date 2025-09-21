import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {Character} from '../character';

export interface CharacterSheetDialogData {
    character: Character;
}

@Component({
    selector: 'app-character-sheet-dialog',
    templateUrl: './character-sheet-dialog.component.html',
    styleUrls: ['./character-sheet-dialog.component.scss', '../shared/full-screen-dialog.scss'],
    standalone: false
})
export class CharacterSheetDialogComponent implements OnInit {

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: CharacterSheetDialogData
    ) {
    }

    ngOnInit() {
    }

}
