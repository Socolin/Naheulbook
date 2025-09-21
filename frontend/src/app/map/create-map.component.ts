import {Component} from '@angular/core';
import {UntypedFormControl, UntypedFormGroup, Validators} from '@angular/forms';

import {requiredFileType} from '../utils/required-file-type.validator';
import {MapService} from './map.service';
import {Map} from './map.model';

@Component({
    selector: 'app-create-map',
    templateUrl: './create-map.component.html',
    styleUrls: ['./create-map.component.scss'],
    standalone: false
})
export class CreateMapComponent {
    public form = new UntypedFormGroup({
        name: new UntypedFormControl(undefined, Validators.required),
        isGm: new UntypedFormControl(false, Validators.required),
        unitName: new UntypedFormControl(undefined, Validators.required),
        pixelPerUnit: new UntypedFormControl(undefined, Validators.required),
        image: new UntypedFormControl(undefined, [Validators.required, requiredFileType(['png', 'jpg', 'jpeg', 'svg', 'bmp'])])
    });
    public attributions: {
        name: string,
        url: string
    }[] = [];

    public uploading = false;
    public progress = 0;
    public createdMap?: Map;

    constructor(
        private readonly mapService: MapService
    ) {

    }

    onSelectFile(event: Event) {
        const inputElement = event.target;
        if (!(inputElement instanceof HTMLInputElement)) {
            return;
        }
        if (!inputElement.files || inputElement.files.length === 0) {
            return;
        }

        const file = inputElement.files[0];
        this.form.controls['image'].setValue(file);
        this.form.controls['image'].markAsTouched();
    }

    addAttribution() {
        this.attributions.push({
            url: '',
            name: ''
        });
    }

    removeAttribution(i: number) {
        this.attributions.splice(i, 1);
    }

    valid() {
        this.uploading = true;
        this.progress = 0;
        this.form.disable();
        this.mapService.createMap({
                name: this.form.value.name,
                data: {
                    isGm: this.form.value.isGm,
                    attribution: this.attributions,
                    unitName: this.form.value.unitName,
                    pixelPerUnit: +this.form.value.pixelPerUnit,
                }
            },
            this.form.value.image,
            (progress) => {
                this.progress = progress
            }
        ).subscribe((map) => {
            this.createdMap = map;
            this.uploading = false;
            this.form.enable();
        }, (error) => {
            this.uploading = false;
            this.form.enable();
            throw error;
        });
    }
}
