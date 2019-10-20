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

    private leafletMap: L.Map;
    private gridLayer?: L.LayerGroup;

    public map?: Map;

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
                this.gridSize = map.data.pixelPerUnit / Math.pow(2, map.data.zoomCount);
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
        const xCoords = this.getGridXs(gridSize, latLngSize.lng);
        const yCoords = this.getGridYs(gridSize, latLngSize.lat);

        for (let {index, x} of xCoords) {
            L.polyline([[0, x], [latLngSize.lat, x]], {
                color: index % 5 === 0 ? '#a44' : '#363636',
                weight: index % 5 === 0 ? 2 : 1,
            }).addTo(grid);
        }

        for (let {index, y} of yCoords) {
            L.polyline([[y, 0], [y, latLngSize.lng]], {
                color: index % 5 === 0 ? '#a44' : '#363636',
                weight: index % 5 === 0 ? 2 : 1,
            }).addTo(grid);
        }

        if (this.isGridDraggable) {
            for (let x of xCoords.filter(e => e.index % 5 === 0).map(e => e.x)) {
                for (let y of yCoords.filter(e => e.index % 5 === 0).map(e => e.y)) {
                    L.circle([y, x], 1, {
                        color: '#000',
                        fillColor: '#000',
                    }).addTo(grid);
                }
            }
        }

        return grid;
    }

    getGridXs(gridSize: number, maxLng: number): { x: number, index: number }[] {
        const xCoords: { x: number, index: number }[] = [];
        const cellOffsetX = Math.floor(this.gridOffsetX / gridSize);
        let visibleIndex = 0;
        for (let i = -5; (i - 5) * gridSize < maxLng; i++) {
            const x = i * gridSize + this.gridOffsetX;
            if (x < 0 || x > maxLng) {
                continue;
            }
            const cellIndex = visibleIndex++ - cellOffsetX;
            xCoords.push({
                x,
                index: cellIndex
            });
        }
        return xCoords;
    }

    getGridYs(gridSize: number, maxLat: number): { y: number, index: number }[] {
        const yCoords: { y: number, index: number }[] = [];
        const cellOffsetY = Math.floor(this.gridOffsetY / gridSize);
        let visibleIndex = 0;
        for (let i = 5; (i + 5) * gridSize > maxLat; i--) {
            const y = i * gridSize - this.gridOffsetY;
            if (y > 0 || y < maxLat) {
                continue;
            }
            const cellIndex = visibleIndex++ - cellOffsetY;
            yCoords.push({
                y,
                index: cellIndex
            });
        }
        return yCoords;
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
        this.gridOffsetX = offsetX % (this.gridSize * 5);
        this.updateGrid();
    }

    changeGridOffsetY(offsetY: number) {
        this.gridOffsetY = offsetY % (this.gridSize * 5);
        this.updateGrid();
    }

    startDragGrid(): void {
        if (!this.gridLayer) {
            return;
        }

        this.isGridDraggable = true;

        if (this.gridDraggable) {
            this.gridDraggable.enable();
            this.updateGrid();
            return;
        }

        this.gridDraggable = new L.Draggable(this.gridLayer.getPane()!);
        this.gridDraggable.enable();
        this.gridDraggable.on('dragend', (event) => {
            this.gridOffsetX += event.target._newPos.x / Math.pow(2, this.leafletMap.getZoom());
            this.gridOffsetX %= this.gridSize * 5;
            this.gridOffsetY += event.target._newPos.y / Math.pow(2, this.leafletMap.getZoom());
            this.gridOffsetY %= this.gridSize * 5;
            event.target._newPos.x = 0;
            event.target._newPos.y = 0;
            (event.target._element as HTMLDivElement).style.transform = 'none';
            this.updateGrid();
        });
        this.updateGrid();
    }

    stopDragGrid() {
        if (!this.gridDraggable) {
            return;
        }
        this.isGridDraggable = false;
        this.gridDraggable.disable();
        this.updateGrid();
    }
}
