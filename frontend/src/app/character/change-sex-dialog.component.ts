import {Component, Inject} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import {CharacterSex} from '../api/shared/enums/character-sex';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatRadioGroup, MatRadioButton } from '@angular/material/radio';
import { FormsModule } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';

export interface ChangeSexDialogData {
    sex: CharacterSex;
}

@Component({
    templateUrl: './change-sex-dialog.component.html',
    styleUrls: ['./change-sex-dialog.component.scss'],
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, MatRadioGroup, FormsModule, MatRadioButton, MatIcon, MatDialogActions, MatButton, MatDialogClose]
})
export class ChangeSexDialogComponent {
    public selectedSex: CharacterSex;

    constructor(
        private readonly dialogRef: MatDialogRef<ChangeSexDialogComponent, CharacterSex>,
        @Inject(MAT_DIALOG_DATA) public readonly data: ChangeSexDialogData,
    ) {
        this.selectedSex = data.sex;
    }

    valid() {
        this.dialogRef.close(this.selectedSex);
    }
}
