import {Component, Inject} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {MapLayer} from './map.model';

export interface MapLayerDialogData {
    layer?: MapLayer
}

export interface MapLayerDialogResult {
    name: string;
    source: 'official' | 'private';
    isGm: boolean
}

@Component({
    templateUrl: './map-layer-dialog.component.html',
    styleUrls: ['./map-layer-dialog.component.scss']
})
export class MapLayerDialogComponent {

    form: FormGroup = new FormGroup({
        'name': new FormControl(undefined, [Validators.required]),
        'source': new FormControl('private', [Validators.required]),
        'isGm': new FormControl(false, [Validators.required]),
    });

    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: MapLayerDialogData,
        private readonly dialogRef: MatDialogRef<MapLayerDialogComponent, MapLayerDialogResult>
    ) {
        if (data.layer) {
            this.form.reset({
                name: data.layer.name,
                source: data.layer.source,
                isGm: data.layer.isGm
            })
        }
    }

    validate() {
        this.dialogRef.close(this.form.value);
    }
}
