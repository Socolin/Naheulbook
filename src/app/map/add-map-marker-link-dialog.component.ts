import {Component, OnInit} from '@angular/core';
import {MapService} from './map.service';
import {MapSummaryResponse} from '../api/responses';
import {FormControl, Validators} from '@angular/forms';
import {Observable, Subscription} from 'rxjs';
import {filter, map, startWith, tap} from 'rxjs/operators';
import {removeDiacritics} from '../shared';
import {Map, MapMarker} from './map.model';
import {MatDialogRef} from '@angular/material/dialog';

export interface AddMapMarkerLinkDialogResult {
    name?: string;
    targetMapId: number;
    targetMapMarkerId?: number;
}

@Component({
    templateUrl: './add-map-marker-link-dialog.component.html',
    styleUrls: ['./add-map-marker-link-dialog.component.scss']
})
export class AddMapMarkerLinkDialogComponent implements OnInit {
    private maps: MapSummaryResponse[];
    public selectedMap?: MapSummaryResponse;
    filteredMaps: Observable<MapSummaryResponse[]>;
    filteredMapMarkers: Observable<MapMarker[]>;
    selectedMapDetails?: Map;

    targetMapFormControl = new FormControl(undefined, Validators.required);
    nameFormControl = new FormControl();
    targetMarkerFormControl = new FormControl();

    private mapDetailSubscription?: Subscription;

    constructor(
        private readonly dialogRef: MatDialogRef<AddMapMarkerLinkDialogComponent, AddMapMarkerLinkDialogResult>,
        private readonly mapService: MapService
    ) {
        this.filteredMaps = this.targetMapFormControl.valueChanges
            .pipe(
                startWith(''),
                tap(v => {
                    if (typeof v !== 'string') {
                        this.selectMap(v);
                    } else {
                        this.selectMap(undefined);
                    }
                }),
                map(value => this.filterAutocomplete(this.maps, value))
            );

        this.filteredMapMarkers = this.targetMarkerFormControl.valueChanges
            .pipe(
                startWith(''),
                filter(() => !!this.selectedMapDetails),
                map(value => {
                    let allMarkers = this.selectedMapDetails
                        ? this.selectedMapDetails.layers.reduce((markers: MapMarker[], l) => markers.concat(l.markers), [])
                        : [];
                    return this.filterAutocomplete(allMarkers, value)
                })
            );
    }

    ngOnInit() {
        this.mapService.getMaps().subscribe(maps => {
            this.maps = maps;
        });
    }

    validate() {
        const targetMap = this.targetMapFormControl.value;
        const targetMapMarker = this.targetMarkerFormControl.value;

        this.dialogRef.close({
            name: this.nameFormControl.value,
            targetMapId: targetMap.id,
            targetMapMarkerId: targetMapMarker ? targetMapMarker.id : undefined
        });
    }

    private selectMap(mapSummary?: MapSummaryResponse) {
        this.selectedMap = mapSummary;
        if (mapSummary) {
            if (this.mapDetailSubscription) {
                this.mapDetailSubscription.unsubscribe();
            }

            this.mapDetailSubscription = this.mapService.getMap(mapSummary.id).subscribe(mapDetails => {
                this.selectedMapDetails = mapDetails;
                this.targetMarkerFormControl.enable();
            });
        } else {
            this.targetMarkerFormControl.disable();
            this.selectedMapDetails = undefined;
        }
    }

    displayMapNameFn(m?: MapSummaryResponse) {
        return m ? m.name : undefined;
    }

    private filterAutocomplete<T extends { name: string }>(elements: T[], value: string | T): T[] {
        if (!value) {
            return elements;
        }
        let mapName: string;
        if (typeof value === 'string') {
            mapName = value;
        } else {
            mapName = value.name;
        }

        const filterValue = removeDiacritics(mapName).toLowerCase();

        return elements.filter(m => removeDiacritics(m.name).toLowerCase().includes(filterValue));
    }
}
