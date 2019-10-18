import {MapData} from '../api/shared';
import {MapResponse} from '../api/responses';
import {LatLng, LatLngBounds} from 'leaflet';

export class Map {
    id: number;
    name: string;
    data: MapData;

    static fromResponse(response: MapResponse): Map {
        const map = new Map();
        map.id = response.id;
        map.name = response.name;
        map.data = response.data;
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
