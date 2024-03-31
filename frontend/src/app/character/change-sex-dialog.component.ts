import {Component, Inject} from '@angular/core';
import {MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA, MatLegacyDialogRef as MatDialogRef} from '@angular/material/legacy-dialog';
import {CharacterSex} from '../api/shared/enums/character-sex';

export interface ChangeSexDialogData {
    sex: CharacterSex;
}

@Component({
    templateUrl: './change-sex-dialog.component.html',
    styleUrls: ['./change-sex-dialog.component.scss']
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
