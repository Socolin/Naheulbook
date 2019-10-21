import {MapData} from '../shared';
import {MapLayerResponse} from './map-layer-response';

export interface MapResponse {
    id: number;
    name: string;
    data: MapData;
    layers: MapLayerResponse[];
}
