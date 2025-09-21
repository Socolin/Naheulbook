import {Component, Inject} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogTitle, MatDialogContent, MatDialogActions, MatDialogClose } from '@angular/material/dialog';
import {MapMarkerType} from './map.model';
import { CdkScrollable } from '@angular/cdk/scrolling';
import { MatRadioGroup, MatRadioButton } from '@angular/material/radio';
import { FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';

export interface SelectMarkerTypeDialogData {
    markerType?: MapMarkerType
}
export interface SelectMarkerTypeDialogResult {
    markerType: MapMarkerType
}

@Component({
    templateUrl: './select-marker-type-dialog.component.html',
    styleUrls: ['./select-marker-type-dialog.component.scss'],
    imports: [MatDialogTitle, CdkScrollable, MatDialogContent, MatRadioGroup, FormsModule, MatRadioButton, MatDialogActions, MatButton, MatDialogClose]
})
export class SelectMarkerTypeDialogComponent {
    types: { value: MapMarkerType, displayName: string }[] = [
        {
            displayName: 'Point',
            value: 'point'
        },
        {
            displayName: 'Zone',
            value: 'area'
        },
        {
            displayName: 'Rectangle',
            value: 'rectangle'
        },
        {
            displayName: 'Cercle',
            value: 'circle'
        }
    ];
    type: MapMarkerType;

    constructor(
        @Inject(MAT_DIALOG_DATA) private readonly data: SelectMarkerTypeDialogData,
        private readonly dialogRef: MatDialogRef<SelectMarkerTypeDialogComponent, SelectMarkerTypeDialogResult>
    ) {
        this.type = data.markerType || 'point';
    }

    validate() {
        this.dialogRef.close({markerType: this.type});
    }

}
