import {MapData} from '../shared';

export interface CreateMapRequest {
    name: string;
    data: Partial<MapData>;
}
