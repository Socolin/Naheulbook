import {MapData} from '../api/shared';
import {MapLayerResponse, MapResponse} from '../api/responses';
import {LatLng, LatLngBounds} from 'leaflet';

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

    getLatLngSize(): LatLng {
        const lat = -this.data.height / Math.pow(2, this.data.zoomCount);
        const lng = this.data.width / Math.pow(2, this.data.zoomCount);
        return new LatLng(lat, lng);
    }

    getBounds(): LatLngBounds {
        const latLngSize = this.getLatLngSize();
        return new LatLngBounds([latLngSize.lat, 0], [0, latLngSize.lng])
    }

    getCenter(): LatLng {
        const latLngSize = this.getLatLngSize();
        return new LatLng(latLngSize.lat / 2, latLngSize.lng / 2)
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
