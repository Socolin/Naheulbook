import {Component} from '@angular/core';
import {MapService} from './map.service';
import {FormControl, FormGroup} from '@angular/forms';

@Component({
    selector: 'app-create-map',
    templateUrl: './create-map.component.html',
    styleUrls: ['./create-map.component.scss']
})
export class CreateMapComponent {
    public form = new FormGroup({
        name: new FormControl(),
        image: new FormControl()
    });
    public attributions: {
        name: string,
        url: string
    }[] = [];

    constructor(
        private readonly mapService: MapService
    ) {

    }

    onMapFileSelect(event: Event) {
        const inputElement = event.target;
        if (!(inputElement instanceof HTMLInputElement)) {
            return;
        }
        if (!inputElement.files || inputElement.files.length === 0) {
            return;
        }

        const file = inputElement.files[0];
        this.mapService.createMap({
            name: this.form.value.name,
            data: {
                attribution: this.attributions,
            }
        }, file).subscribe((result) => {

        });
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
}
