import {Component, ElementRef, HostListener, NgZone, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {MatSidenav} from '@angular/material';
import {MatDialog} from '@angular/material/dialog';

import * as L from 'leaflet';
import {MapService} from './map.service';
import {
    Map,
    MapLayer,
    MapMarker,
    MapMarkerArea,
    MapMarkerCircle,
    MapMarkerLink,
    MapMarkerPoint,
    MapMarkerRectangle,
    MapMarkerType
} from './map.model';
import {MapLayerDialogComponent, MapLayerDialogData, MapLayerDialogResult} from './map-layer-dialog.component';
import {
    SelectMarkerTypeDialogComponent,
    SelectMarkerTypeDialogData,
    SelectMarkerTypeDialogResult
} from './select-marker-type-dialog.component';
import {assertNever} from '../utils/utils';
import {FormControl, FormGroup} from '@angular/forms';
import {MapMarkerRequest} from '../api/requests';
import {BreakpointObserver, Breakpoints} from '@angular/cdk/layout';
import {Subscription} from 'rxjs';
import {LoginService, User} from '../user';
import {
    AddMapMarkerLinkDialogResult,
    MapMarkerLinkDialogComponent,
    MapMarkerLinkDialogData
} from './map-marker-link-dialog.component';

@Component({
    selector: 'app-map',
    templateUrl: './map.component.html',
    styleUrls: ['./map.component.scss']
})
export class MapComponent implements OnInit, OnDestroy {
    @ViewChild('mapElement', {static: true})
    private element: ElementRef;
    @ViewChild('menuSidenav', {static: true})
    private menuSidenav: MatSidenav;
    @ViewChild('infoSidenav', {static: true})
    private infoSidenav: MatSidenav;
    @ViewChild('markerNameInput', {static: false})
    private markerNameInput: ElementRef;

    protected subscription: Subscription = new Subscription();

    private leafletMap: L.Map;
    private gridLayer?: L.LayerGroup;

    public map?: Map;

    public gridDisplayed = false;
    public gridSize = 5;
    public gridOffsetX = 0;
    public gridOffsetY = 0;
    public isGridDraggable: boolean;
    private gridDraggable?: L.Draggable;

    public lastCreatedMarkerType: MapMarkerType;
    public selectedMarker?: MapMarker;
    public selectedLayer?: MapLayer;
    public markerForm = new FormGroup({
        name: new FormControl(),
        description: new FormControl(),
    });
    public expandedLayerList: { [mapLayerId: number]: boolean } = {};
    public hiddenLayers: { [mapLayerId: number]: boolean } = {};
    public isMobile: boolean;
    public currentUser?: User;

    private positionMarker1?: L.Marker;
    private positionMarker2?: L.Marker;
    private measureLine?: L.Polyline;

    constructor(
        private readonly ngZone: NgZone,
        private readonly route: ActivatedRoute,
        private readonly mapService: MapService,
        private readonly dialog: MatDialog,
        private readonly breakpointObserver: BreakpointObserver,
        private readonly loginService: LoginService,
        private readonly router: Router,
    ) {
    }

    ngOnInit() {
        this.subscription.add(this.breakpointObserver.observe([
            Breakpoints.Handset
        ]).subscribe(result => {
            this.isMobile = result.breakpoints[Breakpoints.HandsetPortrait];
        }));

        this.route.paramMap.subscribe(paramMap => {
            const mapId = paramMap.get('mapId');
            if (!mapId) {
                return;
            }
            this.mapService.getMap(+mapId).subscribe(map => {
                this.map = map;
                this.gridSize = map.data.pixelPerUnit / Math.pow(2, map.imageData.zoomCount);
                this.selectedLayer = undefined;
                this.gridDraggable = undefined;
                this.isGridDraggable = false;
                this.createLeafletMap();
                this.map.layers.forEach(l => l.markers.forEach(m => this.addMarkerToMap(m)));
                const targetMarkerId = this.route.snapshot.queryParamMap.get('targetMarkerId');
                if (targetMarkerId) {
                    const marker = this.map.layers
                        .reduce((markers: MapMarker[], l) => markers.concat(l.markers), [])
                        .find(m => m.id === +targetMarkerId);
                    if (marker) {
                        this.goToMarker(marker);
                    }
                }
            })
        });

        this.subscription.add(this.loginService.checkLogged().subscribe((user) => {
            this.currentUser = user;
        }));
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }

    addMarkerToMap(mapMarker: MapMarker) {
        this.ngZone.runOutsideAngular(() => {
            mapMarker.initMarker(this.leafletMap, () => {
                this.ngZone.run(() => {
                    this.selectMarker(mapMarker);
                });
            })
        })
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
        if (this.leafletMap) {
            this.leafletMap.remove();
            this.gridDisplayed = false;
            this.gridLayer = undefined;
            this.gridOffsetX = 0;
            this.gridOffsetY = 0;
        }

        this.ngZone.runOutsideAngular(() => {
            if (!this.map) {
                return;
            }

            const leafletMap = L.map(this.element.nativeElement, {
                crs: L.CRS.Simple,
                minZoom: 0,
                almostOnMouseMove: false,
                maxZoom: this.map.imageData.zoomCount + this.map.imageData.extraZoomCount,
                editable: true
            } as any).setView(this.map.getCenter(), 1);

            leafletMap.createPane('grid');
            leafletMap.zoomControl.setPosition('topright');

            L.tileLayer(`/mapdata/${this.map.id}/{z}/{x}_{y}.png`, {
                attribution: this.map.data.attribution.map(x => `&copy;<a href=${x.url}>${x.name}</a>`).join('|')
            }).addTo(leafletMap);

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

        const grid = L.layerGroup([], {pane: 'grid'});
        const xCoords = this.getGridXs(gridSize, latLngSize.lng);
        const yCoords = this.getGridYs(gridSize, latLngSize.lat);

        for (let {index, x} of xCoords) {
            L.polyline([[0, x], [latLngSize.lat, x]], {
                color: index % 5 === 0 ? '#a44' : '#363636',
                weight: index % 5 === 0 ? 2 : 1,
                pane: 'grid',
            }).addTo(grid);
        }

        for (let {index, y} of yCoords) {
            L.polyline([[y, 0], [y, latLngSize.lng]], {
                color: index % 5 === 0 ? '#a44' : '#363636',
                weight: index % 5 === 0 ? 2 : 1,
                pane: 'grid',
            }).addTo(grid);
        }

        if (this.isGridDraggable) {
            for (let x of xCoords.filter(e => e.index % 5 === 0).map(e => e.x)) {
                for (let y of yCoords.filter(e => e.index % 5 === 0).map(e => e.y)) {
                    L.circle([y, x], 3, {
                        color: '#000',
                        fillColor: '#000',
                        pane: 'grid',
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
        if (this.gridDisplayed && this.gridLayer) {
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

        this.gridDraggable = new L.Draggable(this.gridLayer!.getPane()!);
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

    openAddMapLayerDialog() {
        const map = this.map;
        if (!map) {
            return;
        }
        const dialogRef = this.dialog.open<MapLayerDialogComponent, MapLayerDialogData, MapLayerDialogResult>(
            MapLayerDialogComponent, {
                data: {}
            }
        );

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            this.mapService.createMapLayer(map.id, result).subscribe(mapLayer => {
                map.layers.push(mapLayer)
            });
        })
    }

    @HostListener('window:keydown', ['$event'])
    keyEvent(event: KeyboardEvent) {
        if (!this.selectedLayer) {
            return;
        }
        if (this.infoSidenav.opened) {
            return;
        }
        if (this.menuSidenav.opened) {
            return;
        }

        switch (event.key) {
            case 'c':
                this.addNewMapMarker(this.selectedLayer, 'circle');
                break;
            case 'r':
                this.addNewMapMarker(this.selectedLayer, 'rectangle');
                break;
            case 'z':
                this.addNewMapMarker(this.selectedLayer, 'area');
                break;
            case 'p':
                this.addNewMapMarker(this.selectedLayer, 'point');
                break;
        }
    }


    startAddMapMarker(mapLayer: MapLayer) {
        const dialogRef = this.dialog.open<SelectMarkerTypeDialogComponent, SelectMarkerTypeDialogData, SelectMarkerTypeDialogResult>(
            SelectMarkerTypeDialogComponent,
            {
                autoFocus: false,
                data: {
                    markerType: this.lastCreatedMarkerType
                }
            });

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            this.addNewMapMarker(mapLayer, result.markerType);
        });
    }

    private addNewMapMarker(mapLayer: MapLayer, markerType: MapMarkerType) {
        this.lastCreatedMarkerType = markerType;

        const lastColor = mapLayer.markers.reduce((color, m) => m.getColor() || color, '#00a7ff');
        this.ngZone.runOutsideAngular(() => {
            let marker: MapMarker;
            switch (markerType) {
                case 'point': {
                    marker = new MapMarkerPoint(mapLayer, this.leafletMap.getCenter());
                    break;
                }
                case 'area': {
                    const bounds = this.leafletMap.getBounds();
                    const height = bounds.getSouth() - bounds.getNorth();
                    const width = bounds.getEast() - bounds.getWest();
                    const points = [
                        L.latLng([bounds.getSouth() - height * 0.3, bounds.getWest() + width * 0.3]),
                        L.latLng([bounds.getSouth() - height * 0.3, bounds.getEast() - width * 0.3]),
                        L.latLng([bounds.getNorth() + height * 0.3, bounds.getEast() - width * 0.3]),
                        L.latLng([bounds.getNorth() + height * 0.3, bounds.getWest() + width * 0.3])
                    ];
                    marker = new MapMarkerArea(mapLayer, points);
                    marker.color = lastColor;
                    break;
                }
                case 'rectangle': {
                    const bounds = this.leafletMap.getBounds();
                    const height = bounds.getSouth() - bounds.getNorth();
                    const width = bounds.getEast() - bounds.getWest();
                    marker = new MapMarkerRectangle(mapLayer, [
                        [bounds.getSouth() - height * 0.3, bounds.getWest() + width * 0.3],
                        [bounds.getNorth() + height * 0.3, bounds.getEast() - width * 0.3],
                    ]);
                    marker.color = lastColor;
                    break;
                }
                case 'circle': {
                    marker = new MapMarkerCircle(mapLayer, this.leafletMap.getCenter(), 50 / Math.pow(2, this.leafletMap.getZoom()));
                    marker.color = lastColor;
                    break;
                }
                default:
                    assertNever(markerType);
                    throw new Error('Invalid marker type');
            }

            marker.editable = true;
            marker.name = 'Nouveau marqueur';
            this.addMarkerToMap(marker);
            if (!this.isMobile) {
                this.selectMarker(marker);
                this.infoSidenav.open().then(() => {
                    this.markerNameInput.nativeElement.focus();
                });
            }
        });

        this.menuSidenav.close();
    }


    cancelEdit(mapMarker: MapMarker) {
        mapMarker.setEditable(false);
        if (mapMarker === this.selectedMarker) {
            this.selectedMarker = undefined;
            this.infoSidenav.close();
        }
        if (!mapMarker.id) {
            mapMarker.leafletMarker!.remove();
        }
    }

    saveMarker(mapMarker: MapMarker) {
        const request: MapMarkerRequest = {
            ...this.markerForm.value,
            type: mapMarker.type,
        };
        request.markerInfo = mapMarker.getMarkerInfo();
        if (mapMarker.type === 'rectangle' || mapMarker.type === 'circle' || mapMarker.type === 'area') {
            request.markerInfo.color = mapMarker.leafletMarker!.options.color;
        }

        if (mapMarker.id) {
            this.mapService.editMarker(mapMarker.mapLayer, mapMarker.id, request).subscribe(newMapMarker => {
                mapMarker.remove();
                const i = mapMarker.mapLayer.markers.indexOf(mapMarker);
                mapMarker.mapLayer.markers[i] = newMapMarker;
                this.addMarkerToMap(newMapMarker);
                this.infoSidenav.close();
            });
        } else {
            this.mapService.createMarker(mapMarker.mapLayer, request).subscribe(newMapMarker => {
                mapMarker.remove();
                mapMarker.mapLayer.markers.push(newMapMarker);
                this.addMarkerToMap(newMapMarker);
                this.infoSidenav.close();
            });
        }
    }

    changeSelectedMarkerColor(event: string) {
        if (!this.selectedMarker) {
            return;
        }
        if (this.selectedMarker.type !== 'area' && this.selectedMarker.type !== 'circle' && this.selectedMarker.type !== 'rectangle') {
            return;
        }
        this.selectedMarker.leafletMarker!.setStyle({
            color: event
        });
    }

    private selectMarker(marker: MapMarker) {
        if (this.selectedMarker !== marker) {
            this.markerForm.reset({
                name: marker.name,
                description: marker.description
            });
            this.selectedMarker = marker;
        }
        this.infoSidenav.open();
    }

    deleteMarker(mapMarker: MapMarker) {
        mapMarker.setEditable(false);
        this.mapService.deleteMarker(mapMarker.id!).subscribe(() => {
            mapMarker.remove();
            this.infoSidenav.close();
        });
    }

    startEditMarker(mapMarker: MapMarker) {
        mapMarker.setEditable(true);
    }

    expandMarkerList(mapLayer: MapLayer) {
        this.expandedLayerList[mapLayer.id] = true;
    }

    collapseMarkerList(mapLayer: MapLayer) {
        delete this.expandedLayerList[mapLayer.id];
    }

    toggleVisibility(mapLayer: MapLayer) {
        const hidden = !this.hiddenLayers[mapLayer.id];
        this.hiddenLayers[mapLayer.id] = hidden;
        if (hidden) {
            mapLayer.markers.forEach(m => m.remove());
        } else {
            mapLayer.markers.forEach(m => this.addMarkerToMap(m));
        }
    }

    canEditLayer(mapLayer: MapLayer) {
        if (!this.currentUser) {
            return false;
        }
        if (mapLayer.source === 'official') {
            return this.currentUser.admin;
        }
        return true;
    }

    goToMarker(marker: MapMarker) {
        if (marker.leafletMarker) {
            this.leafletMap.setView(marker.getCenter(), this.map!.imageData.zoomCount);
        }
    }

    editMap() {
        this.router.navigate(['/map', 'edit', this.map!.id]);
    }

    startAddMapMarkerLink(mapMarker: MapMarker) {
        const dialogRef = this.dialog.open<MapMarkerLinkDialogComponent, MapMarkerLinkDialogData, AddMapMarkerLinkDialogResult>(
            MapMarkerLinkDialogComponent, {
                autoFocus: false,
                data: {}
            });

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            this.mapService.createMapMarkerLink(mapMarker.id!, {
                name: result.name,
                targetMapId: result.targetMapId,
                targetMapMarkerId: result.targetMapMarkerId
            }).subscribe(link => {
                mapMarker.links.push(link);
            })
        });
    }

    goToMap(targetMapId: number, targetMapMarkerId?: number) {
        this.router.navigate(['/map', targetMapId], {queryParams: {targetMarkerId: targetMapMarkerId}});
        this.infoSidenav.close();
    }

    selectLayer(mapLayer: MapLayer) {
        this.selectedLayer = mapLayer;
    }

    deleteLink(mapMarker: MapMarker, link: MapMarkerLink) {
        this.mapService.deleteLink(link.id).subscribe(() => {
            const index = mapMarker.links.indexOf(link);
            if (index !== -1) {
                mapMarker.links.splice(index, 1);
            }
        });
    }

    startEditLink(mapMarker: MapMarker, link: MapMarkerLink) {
        const dialogRef = this.dialog.open<MapMarkerLinkDialogComponent, MapMarkerLinkDialogData, AddMapMarkerLinkDialogResult>(
            MapMarkerLinkDialogComponent, {
                autoFocus: false,
                data: {
                    link: link
                }
            });

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            this.mapService.editMapMarkerLink(link.id, {
                name: result.name,
                targetMapId: result.targetMapId,
                targetMapMarkerId: result.targetMapMarkerId
            }).subscribe(editedLink => {
                const index = mapMarker.links.indexOf(link);
                if (index !== -1) {
                    mapMarker.links[index] = editedLink;
                }
            })
        });
    }

    deleteLayer(mapLayer: MapLayer) {
        this.mapService.deleteMapLayer(mapLayer.id).subscribe(() => {
            const index = this.map!.layers.findIndex(l => l.id === mapLayer.id);
            if (index !== -1) {
                this.map!.layers.splice(index, 1);
            }
        });
    }

    startEditLayer(mapLayer: MapLayer) {
        const map = this.map;
        if (!map) {
            return;
        }
        const dialogRef = this.dialog.open<MapLayerDialogComponent, MapLayerDialogData, MapLayerDialogResult>(
            MapLayerDialogComponent, {
                data: {
                    layer: mapLayer
                }
            }
        );

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            this.mapService.editMapLayer(mapLayer.id, result).subscribe(editedMapLayer => {
                const index = map.layers.indexOf(mapLayer);
                if (index !== -1) {
                    map.layers[index] = editedMapLayer;
                }
            });
        })
    }

    toggleMeasure(active: boolean) {
        if (!active) {
            if (this.positionMarker1) {
                this.positionMarker1.remove();
            }
            if (this.positionMarker2) {
                this.positionMarker2.remove();
            }
            if (this.measureLine) {
                this.measureLine.remove();
            }
            this.positionMarker1 = undefined;
            this.positionMarker2 = undefined;
            this.measureLine = undefined;
            return;
        }
        this.menuSidenav.close();
        const bounds = this.leafletMap.getBounds();
        const height = bounds.getSouth() - bounds.getNorth();
        const width = bounds.getEast() - bounds.getWest();
        let position1 = L.latLng(bounds.getNorth() + height / 2, bounds.getEast() - width * 0.3);
        let position2 = L.latLng(bounds.getNorth() + height / 2, bounds.getWest() + width * 0.3);
        this.positionMarker1 = L.marker(position1, {draggable: true})
            .addTo(this.leafletMap);
        this.positionMarker2 = L.marker(position2, {draggable: true})
            .addTo(this.leafletMap);
        this.measureLine = L.polyline([position1, position2])
            .addTo(this.leafletMap);

        this.positionMarker1.on('drag', this.updateMeasureLine.bind(this));
        this.positionMarker2.on('drag', this.updateMeasureLine.bind(this));
        this.positionMarker1.on('dragend', this.updateMeasureLine.bind(this));
        this.positionMarker2.on('dragend', this.updateMeasureLine.bind(this));
        this.updateMeasureLine();
    }

    private updateMeasureLine() {
        if (this.measureLine && this.positionMarker1 && this.positionMarker2) {
            this.measureLine.setLatLngs(
                [this.positionMarker1.getLatLng(), this.positionMarker2.getLatLng()]
            );

            let unitName = this.map!.data.unitName;
            const result = /(\d+)(.+)/.exec(unitName);
            let unitCount = 1;
            if (result) {
                unitCount = +result[1];
                unitName = result[2];
            }

            let distance = L.CRS.Simple.distance(
                this.positionMarker1.getLatLng(),
                this.positionMarker2.getLatLng()
            ) * Math.pow(2, this.map!.imageData.zoomCount) / (this.map!.data.pixelPerUnit / unitCount);

            let label = distance.toFixed(2) + unitName;
            this.measureLine.unbindTooltip();
            this.measureLine.bindTooltip(label, {
                permanent: true,
                direction: 'right'
            });
        }
    }
}
