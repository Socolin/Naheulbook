import {Component, OnInit} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {MapService} from './map.service';

@Component({
    templateUrl: './edit-map.component.html',
    styleUrls: ['./edit-map.component.scss']
})
export class EditMapComponent implements OnInit {
    public saving = false;
    public loading = true;
    public form = new FormGroup({
        name: new FormControl(undefined, Validators.required),
        unitName: new FormControl(undefined, Validators.required),
        pixelPerUnit: new FormControl(undefined, Validators.required),
    });

    public attributions: {
        name: string,
        url: string
    }[] = [];
    public mapId: number;

    constructor(
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly mapService: MapService,
    ) {
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

    ngOnInit() {
        this.route.params.subscribe((params) => {
            this.loading = true;
            this.mapService.getMap(+params['mapId']).subscribe((map) => {
                this.mapId = +params['mapId'];
                this.form.reset({
                    name: map.name,
                    unitName: map.data.unitName,
                    pixelPerUnit: map.data.pixelPerUnit
                });

                this.attributions = [...map.data.attribution];

                this.loading = false;
            });
        });
    }

    valid() {
        this.form.disable();
        this.saving = true;
        this.mapService.editMap(this.mapId, {
            name: this.form.value.name,
            data: {
                attribution: this.attributions,
                unitName: this.form.value.unitName,
                pixelPerUnit: +this.form.value.pixelPerUnit,
            }
        }).subscribe((map) => {
            this.saving = false;
            this.form.enable();
            this.router.navigate(['/map', map.id])
        }, (error) => {
            this.saving = false;
            this.form.enable();
            throw error;
        });
    }
}
