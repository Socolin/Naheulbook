import {Component} from '@angular/core';
import {MatDialogRef} from '@angular/material/dialog';
import {MapMarkerType} from './map.model';

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
    type: MapMarkerType = 'point';

    constructor(
        private readonly dialogRef: MatDialogRef<SelectMarkerTypeDialogComponent, SelectMarkerTypeDialogResult>
    ) {
    }

    validate() {
        this.dialogRef.close({markerType: this.type});
    }

}
