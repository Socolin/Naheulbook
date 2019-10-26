import {MapData} from '../api/shared';
import {MapLayerResponse, MapMarkerResponse, MapResponse} from '../api/responses';

import * as L from 'leaflet';
import {assertNever} from '../utils/utils';
import {LatLng} from 'leaflet';

export class Map {
    id: number;
    name: string;
    data: MapData;
    layers: MapLayer[];

    static fromResponse(response: MapResponse): Map {
        const map = new Map();
        map.id = response.id;
        map.name = response.name;
        map.data = response.data;
        map.layers = response.layers.map(x => MapLayer.fromResponse(x));
        return map;
    }

    getLatLngSize(): L.LatLng {
        const lat = -this.data.height / Math.pow(2, this.data.zoomCount);
        const lng = this.data.width / Math.pow(2, this.data.zoomCount);
        return new L.LatLng(lat, lng);
    }

    getBounds(): L.LatLngBounds {
        const latLngSize = this.getLatLngSize();
        return new L.LatLngBounds([latLngSize.lat, 0], [0, latLngSize.lng])
    }

    getCenter(): L.LatLng {
        const latLngSize = this.getLatLngSize();
        return new L.LatLng(latLngSize.lat / 2, latLngSize.lng / 2)
    }
}

export class MapLayer {
    id: number;
    name: string;
    source: 'official' | 'private';
    markers: MapMarker[];

    static fromResponse(response: MapLayerResponse): MapLayer {
        const mapLayer = new MapLayer();
        mapLayer.id = response.id;
        mapLayer.name = response.name;
        mapLayer.source = response.source;
        mapLayer.markers = response.markers.map(m => MapMarkerBase.fromResponse(mapLayer, m));
        return mapLayer;
    }
}

export type MapMarkerType = 'point' | 'area' | 'rectangle' | 'circle';

const defaultMarkerIcon = new L.Icon({
    iconUrl: '/assets/icons/position-marker.svg',
    className: 'marker-blue',
    iconSize: [36, 36],
    iconAnchor: [16, 36],
    attribution: 'https://game-icons.net'
});

export interface MarkerInfoPoint {
    position: L.LatLngLiteral;
    icon?: string;
}
export interface MarkerInfoRectangle {
    bounds: L.LatLngExpression[],
    color?: string;
}
export interface MarkerInfoArea {
    points: L.LatLngLiteral[]
    color?: string;
}
export interface MarkerInfoCircle {
    center: L.LatLngLiteral;
    radius: number;
    color?: string;
}

export type MarkerInfo = MarkerInfoPoint | MarkerInfoRectangle | MarkerInfoArea | MarkerInfoCircle;

export abstract class MapMarkerBase {
    readonly type: MapMarkerType;
    readonly mapLayer: MapLayer;
    id?: number;
    name: string;
    leafletMarker?: L.Layer;
    editable: boolean;
    description?: string;

    static fromResponse(mapLayer: MapLayer, response: MapMarkerResponse): MapMarker {
        let marker: MapMarker;
        switch (response.type) {
            case 'point': {
                marker = new MapMarkerPoint(mapLayer, response.markerInfo.position);
                break;
            }
            case 'area': {
                marker = new MapMarkerArea(mapLayer, response.markerInfo.points);
                marker.color = response.markerInfo.color;
                break;
            }
            case 'rectangle': {
                marker = new MapMarkerRectangle(mapLayer, response.markerInfo.bounds);
                marker.color = response.markerInfo.color;
                break;
            }
            case 'circle': {
                marker = new MapMarkerCircle(mapLayer, response.markerInfo.center, response.markerInfo.radius);
                marker.color = response.markerInfo.color;
                break;
            }
            default:
                assertNever(response.type);
                throw new Error('Invalid mapMarkerType');
        }
        marker.id = response.id;
        marker.name = response.name;
        marker.description = response.description;
        return marker;
    }

    protected constructor(mapLayer: MapLayer) {
        this.mapLayer = mapLayer;
    }

    public remove() {
        if (this.leafletMarker) {
            this.leafletMarker.remove();
        }
    }

    public initMarker(leafletMap: L.Map, onClick: (event: L.LeafletEvent) => void): void {
        if (!this.leafletMarker) {
            this.leafletMarker = this.createLeafletLayer();
        }
        this.leafletMarker.addTo(leafletMap);
        this.leafletMarker.on('click', onClick);
        this.setMarkerEditable(this.editable);
    }

    public setEditable(editable: boolean): void {
        this.editable = editable;
        this.setMarkerEditable(editable);
    }

    protected abstract createLeafletLayer(): L.Layer;

    public abstract setMarkerEditable(editable: boolean): void;

    public abstract getMarkerInfo(): MarkerInfo;

    public abstract useColor(): boolean;

    public getColor(): string | undefined {
        const markerInfo = this.getMarkerInfo();

        if ('color' in markerInfo) {
            return markerInfo.color;
        }

        return undefined;
    }
}

function serializeLatLng(latLng: L.LatLng): L.LatLngLiteral {
    return {
        lat: latLng.lat,
        lng: latLng.lng
    }
}

export class MapMarkerPoint extends MapMarkerBase {
    readonly type: 'point' = 'point';
    position: L.LatLng;
    icon?: string;
    leafletMarker?: L.Marker;

    constructor(mapLayer: MapLayer, position: L.LatLngLiteral) {
        super(mapLayer);
        this.position = L.latLng(position);
    }

    protected createLeafletLayer(): L.Layer {
        return L.marker(this.position, {icon: defaultMarkerIcon, draggable: this.editable});
    }

    public setMarkerEditable(editable: boolean): void {
        if (editable) {
            this.leafletMarker!.dragging!.enable();
        } else {
            this.leafletMarker!.dragging!.disable();
        }
    }

    public getMarkerInfo(): MarkerInfoPoint {
        return {
            position: this.leafletMarker ? serializeLatLng(this.leafletMarker.getLatLng()) : serializeLatLng(this.position),
            icon: this.icon
        }
    }

    useColor(): boolean {
        return false;
    }
}

export class MapMarkerArea extends MapMarkerBase {
    readonly type: 'area' = 'area';
    points: L.LatLng[];
    leafletMarker?: L.Polygon;
    color?: string;

    constructor(mapLayer: MapLayer, points: L.LatLngLiteral[]) {
        super(mapLayer);
        this.points = points.map(L.latLng);
    }

    protected createLeafletLayer(): L.Layer {
        return L.polygon(this.points, {color: this.color});
    }

    public setMarkerEditable(editable: boolean): void {
        if (editable) {
            (this.leafletMarker as any).enableEdit();
        } else {
            (this.leafletMarker as any).disableEdit();
        }
    }

    public getMarkerInfo(): MarkerInfoArea {
        return {
            points: this.leafletMarker
                ? (this.leafletMarker.getLatLngs()[0] as LatLng[]).map(serializeLatLng)
                : this.points.map(serializeLatLng),
            color: this.color
        }
    }

    useColor(): boolean {
        return true;
    }
}

export class MapMarkerCircle extends MapMarkerBase {
    readonly type: 'circle' = 'circle';
    center: L.LatLng;
    radius: number;
    leafletMarker?: L.Circle;
    color?: string;

    constructor(mapLayer: MapLayer, center: L.LatLngLiteral, radius: number) {
        super(mapLayer);
        this.center = L.latLng(center);
        this.radius = radius;
    }

    protected createLeafletLayer(): L.Layer {
        return L.circle(this.center, this.radius, {color: this.color});
    }

    public setMarkerEditable(editable: boolean): void {
        if (editable) {
            (this.leafletMarker as any).enableEdit();
        } else {
            (this.leafletMarker as any).disableEdit();
        }
    }

    public getMarkerInfo(): MarkerInfoCircle {
        return {
            center: this.leafletMarker ? serializeLatLng(this.leafletMarker.getLatLng()) : serializeLatLng(this.center),
            radius: this.leafletMarker!.getRadius() || this.radius,
            color: this.color
        }
    }

    useColor(): boolean {
        return true;
    }
}

export class MapMarkerRectangle extends MapMarkerBase {
    readonly type: 'rectangle' = 'rectangle';
    bounds: L.LatLngBounds;
    leafletMarker?: L.Rectangle;
    color?: string;

    constructor(mapLayer: MapLayer, bounds: L.LatLngBoundsLiteral) {
        super(mapLayer);
        this.bounds = L.latLngBounds(bounds);
    }

    protected createLeafletLayer(): L.Layer {
        return L.rectangle(this.bounds, {color: this.color});
    }

    public setMarkerEditable(editable: boolean): void {
        if (editable) {
            (this.leafletMarker as any).enableEdit();
        } else {
            (this.leafletMarker as any).disableEdit();
        }
    }

    public getMarkerInfo(): MarkerInfoRectangle {
        return {
            bounds: this.leafletMarker
                ? [
                    serializeLatLng(this.leafletMarker.getBounds().getSouthWest()),
                    serializeLatLng(this.leafletMarker.getBounds().getNorthEast())
                ]
                : [
                    serializeLatLng(this.bounds.getSouthWest()),
                    serializeLatLng(this.bounds.getNorthEast())
                ],
            color: this.color
        }
    }

    useColor(): boolean {
        return true;
    }
}

export type MapMarker = MapMarkerPoint | MapMarkerRectangle | MapMarkerArea | MapMarkerCircle;
