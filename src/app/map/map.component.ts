import {Component, ElementRef, NgZone, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {MatSidenav} from '@angular/material';

import * as L from 'leaflet';
import {MapService} from './map.service';
import {Map} from './map.model';

@Component({
    selector: 'app-map',
    templateUrl: './map.component.html',
    styleUrls: ['./map.component.scss']
})
export class MapComponent implements OnInit {
    @ViewChild('mapElement', {static: true})
    private element: ElementRef;
    @ViewChild('menuSidenav', {static: true})
    private menuSidenav: MatSidenav;
    @ViewChild('infoSidenav', {static: true})
    private infoSidenav: MatSidenav;

    private map?: Map;

    private leafletMap: L.Map;
    private gridLayer?: L.LayerGroup;

    public gridDisplayed = false;
    public gridSize = 5;
    public gridOffsetX = 0;
    public gridOffsetY = 0;

    constructor(
        private readonly ngZone: NgZone,
        private readonly route: ActivatedRoute,
        private readonly mapService: MapService
    ) {
        console.log(mapService);
    }

    ngOnInit() {
        this.route.paramMap.subscribe(paramMap => {
            const mapId = paramMap.get('mapId');
            if (!mapId) {
                return;
            }
            this.mapService.getMap(+mapId).subscribe(map => {
                this.map = map;
                this.gridSize = map.data.pixelPerUnit;
                this.createLeafletMap();
            })
        });
    }

    toggleGrid(checked: boolean) {
        if (!this.gridLayer) {
            this.gridLayer = this.drawGrid();
        }
        this.gridDisplayed = checked;
        if (!this.gridLayer) {
            return;
        }
        if (this.gridDisplayed) {
            this.gridLayer.addTo(this.leafletMap);
        } else {
            this.gridLayer.removeFrom(this.leafletMap);
        }
    }

    private createLeafletMap() {
        this.ngZone.runOutsideAngular(() => {
            if (!this.map) {
                return;
            }

            const leafletMap = L.map(this.element.nativeElement, {
                crs: L.CRS.Simple,
                minZoom: 0,
                maxZoom: 4,
            }).setView(this.map.getCenter(), 1);

            L.tileLayer(`/mapdata/${this.map.id}/{z}/{x}_{y}.png`, {
                attribution: this.map.data.attribution.map(x => `&copy;<a href=${x.url}>${x.name}</a>`).join('|')
            }).addTo(leafletMap);

            const icon = new L.Icon({
                iconUrl: '/assets/icons/position-marker.svg',
                className: 'marker-blue',
                iconSize: [36, 36],
                iconAnchor: [16, 36],
                attribution: 'https://game-icons.net'
            });
            const a = L.marker({lat: -135, lng: 186}, {draggable: true, icon: icon}).addTo(leafletMap);
            a.on('click', event => {
                console.log(event);
                this.ngZone.run(() => {
                    this.infoSidenav.open();
                });
            });

            leafletMap.invalidateSize({});
            this.leafletMap = leafletMap;
        });
    }

    private drawGrid(): L.LayerGroup | undefined {
        const latLngSize = this.map!.getLatLngSize();
        const gridSize = this.gridSize;
        if (gridSize <= 0) {
            return undefined;
        }
        const grid = L.layerGroup();
        for (let i = 0; i * gridSize < latLngSize.lng; i++) {
            const x = i * gridSize + this.gridOffsetX % this.gridSize;
            L.polyline([[0, x], [latLngSize.lat, x]], {
                color: i % 5 === 0 ? '#a44' : '#363636',
                weight: i % 5 === 0 ? 2 : 1,
            }).addTo(grid);
        }
        for (let i = 0; i * gridSize > latLngSize.lat; i--) {
            const y = i * gridSize - this.gridOffsetY % this.gridSize;
            L.polyline([[y, 0], [y, latLngSize.lng]], {
                color: i % 5 === 0 ? '#a44' : '#363636',
                weight: i % 5 === 0 ? 2 : 1,
            }).addTo(grid);
        }
        return grid;
    }

    updateGrid() {
        if (this.gridLayer) {
            this.gridLayer.remove();
        }
        this.gridLayer = this.drawGrid();
        if (this.gridLayer && this.gridDisplayed) {
            this.gridLayer.addTo(this.leafletMap);
        }
    }

    changeGridSize(newSize: number) {
        this.gridSize = newSize;
        this.updateGrid();
    }

    changeGridOffsetX(offsetX: number) {
        this.gridOffsetX = offsetX;
        this.updateGrid();
    }

    changeGridOffsetY(offsetY: number) {
        this.gridOffsetY = offsetY;
        this.updateGrid();
    }
}
