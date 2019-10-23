import {MapData} from '../api/shared';
import {MapLayerResponse, MapResponse} from '../api/responses';

import * as L from 'leaflet';

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

    static fromResponse(response: MapLayerResponse): MapLayer {
        const mapLayer = new MapLayer();
        mapLayer.id = response.id;
        mapLayer.name = response.name;
        mapLayer.source = response.source;
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

export abstract class IMapMarker {
    readonly type: MapMarkerType;
    readonly mapLayer: MapLayer;
    name: string;
    leafletMarker?: L.Layer;
    editable: boolean;

    protected constructor(mapLayer: MapLayer) {
        this.mapLayer = mapLayer;
    }

    initMarker(leafletMap: L.Map, onClick: (event: L.LeafletEvent) => void): void {
        if (!this.leafletMarker) {
            this.leafletMarker = this.createLeafletLayer();
        }
        this.leafletMarker.addTo(leafletMap);
        this.leafletMarker.on('click', onClick);
        this.setEditable(this.editable);
    }

    protected abstract createLeafletLayer(): L.Layer;
    public abstract setEditable(editable: boolean): void;
}

export class MapMarkerPoint extends IMapMarker {
    readonly type: 'point' = 'point';
    position: L.LatLng;
    icon: string;
    leafletMarker?: L.Marker;

    constructor(mapLayer: MapLayer, position: L.LatLng) {
        super(mapLayer);
        this.position = position;
    }

    protected createLeafletLayer(): L.Layer {
        return L.marker(this.position, {icon: defaultMarkerIcon, draggable: this.editable});
    }

    public setEditable(editable: boolean): void {
        if (editable) {
            this.leafletMarker!.dragging!.enable();
        } else {
            this.leafletMarker!.dragging!.disable();
        }
    }
}

export class MapMarkerArea extends IMapMarker {
    readonly type: 'area' = 'area';
    points: L.LatLng[];
    leafletMarker?: L.Polygon;

    constructor(mapLayer: MapLayer, points: L.LatLng[]) {
        super(mapLayer);
        this.points = points;
    }

    protected createLeafletLayer(): L.Layer {
        return L.polygon(this.points, {});
    }

    public setEditable(editable: boolean): void {
        if (editable) {
            (this.leafletMarker as any).enableEdit();
        } else {
            (this.leafletMarker as any).disableEdit();
        }
    }
}

export class MapMarkerCircle extends IMapMarker {
    readonly type: 'circle' = 'circle';
    center: L.LatLng;
    radius: number;
    leafletMarker?: L.Circle;

    constructor(mapLayer: MapLayer, center: L.LatLng, radius: number) {
        super(mapLayer);
        this.center = center;
        this.radius = radius;
    }

    protected createLeafletLayer(): L.Layer {
        return L.circle(this.center, this.radius, {});
    }

    public setEditable(editable: boolean): void {
        if (editable) {
            (this.leafletMarker as any).enableEdit();
        } else {
            (this.leafletMarker as any).disableEdit();
        }
    }
}

export class MapMarkerRectangle extends IMapMarker {
    readonly type: 'rectangle' = 'rectangle';
    bounds: L.LatLngBounds;
    leafletMarker?: L.Rectangle;

    constructor(mapLayer: MapLayer, bounds: L.LatLngBounds) {
        super(mapLayer);
        this.bounds = bounds;
    }

    protected createLeafletLayer(): L.Layer {
        return L.rectangle(this.bounds, {});
    }

    public setEditable(editable: boolean): void {
        if (editable) {
            (this.leafletMarker as any).enableEdit();
        } else {
            (this.leafletMarker as any).disableEdit();
        }
    }
}

export type MapMarker = MapMarkerPoint;
