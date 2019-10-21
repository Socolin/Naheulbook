import {Component} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {MatDialogRef} from '@angular/material/dialog';

export interface AddMapLayerDialogResult {
    name: string;
    source: 'official' | 'private';
}

@Component({
    templateUrl: './add-map-layer-dialog.component.html',
    styleUrls: ['./add-map-layer-dialog.component.scss']
})
export class AddMapLayerDialogComponent {

    form: FormGroup = new FormGroup({
        'name': new FormControl(undefined, [Validators.required]),
        'source': new FormControl('private', [Validators.required])
    });

    constructor(
        private readonly dialogRef: MatDialogRef<AddMapLayerDialogComponent, AddMapLayerDialogResult>
    ) {
    }

    validate() {
        this.dialogRef.close(this.form.value);
    }
}
