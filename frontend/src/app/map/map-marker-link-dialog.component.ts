import {Component, Inject, OnInit} from '@angular/core';
import {MapService} from './map.service';
import {MapSummaryResponse} from '../api/responses';
import {UntypedFormControl, Validators} from '@angular/forms';
import {Observable, Subscription} from 'rxjs';
import {filter, map, startWith, tap} from 'rxjs/operators';
import {removeDiacritics} from '../shared';
import {Map, MapMarker, MapMarkerLink} from './map.model';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';

export interface MapMarkerLinkDialogData {
    link?: MapMarkerLink;
}

export interface AddMapMarkerLinkDialogResult {
    name?: string;
    targetMapId: number;
    targetMapMarkerId?: number;
}

@Component({
    templateUrl: './map-marker-link-dialog.component.html',
    styleUrls: ['./map-marker-link-dialog.component.scss']
})
export class MapMarkerLinkDialogComponent implements OnInit {
    maps?: MapSummaryResponse[];
    selectedMap?: MapSummaryResponse;
    filteredMaps: Observable<MapSummaryResponse[]>;
    filteredMapMarkers: Observable<MapMarker[]>;
    selectedMapDetails?: Map;

    targetMapFormControl = new UntypedFormControl(undefined, Validators.required);
    nameFormControl = new UntypedFormControl();
    targetMarkerFormControl = new UntypedFormControl();

    private mapDetailSubscription?: Subscription;

    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: MapMarkerLinkDialogData,
        private readonly dialogRef: MatDialogRef<MapMarkerLinkDialogComponent, AddMapMarkerLinkDialogResult>,
        private readonly mapService: MapService
    ) {
        this.filteredMapMarkers = this.targetMarkerFormControl.valueChanges
            .pipe(
                startWith<string | MapMarker>(''),
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
            if (this.data.link) {
                const currentLinkMap = maps.find(m => m.id === this.data.link!.targetMapId);
                this.targetMapFormControl.setValue(currentLinkMap);
            } else {
                this.targetMapFormControl.setValue('');
            }
            this.filteredMaps = this.targetMapFormControl.valueChanges
                .pipe(
                    startWith<string | MapSummaryResponse>(this.targetMapFormControl.value || ''),
                    tap(v => {
                        if (typeof v !== 'string') {
                            this.selectMap(v);
                        } else {
                            this.selectMap(undefined);
                        }
                    }),
                    map(value => this.filterAutocomplete(this.maps!, value))
                );

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
                if (this.data.link && this.data.link.targetMapId === mapDetails.id) {
                    const marker = this.selectedMapDetails.layers
                        .reduce((markers: MapMarker[], l) => markers.concat(l.markers), [])
                        .find(m => m.id === this.data.link!.targetMapMarkerId);
                    this.targetMarkerFormControl.setValue(marker);
                }
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
