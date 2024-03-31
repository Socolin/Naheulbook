import {Component, Inject} from '@angular/core';
import {MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA, MatLegacyDialogRef as MatDialogRef} from '@angular/material/legacy-dialog';
import {MapMarkerType} from './map.model';

export interface SelectMarkerTypeDialogData {
    markerType?: MapMarkerType
}
export interface SelectMarkerTypeDialogResult {
    markerType: MapMarkerType
}

@Component({
    templateUrl: './select-marker-type-dialog.component.html',
    styleUrls: ['./select-marker-type-dialog.component.scss']
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
