import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {MapLayerResponse, MapResponse} from '../api/responses';
import {CreateMapLayerRequest, CreateMapRequest} from '../api/requests';
import {map} from 'rxjs/operators';
import {Map, MapLayer} from './map.model';

import {toResponseBody, uploadProgress} from '../utils/operators';

@Injectable()
export class MapService {
    constructor(private httpClient: HttpClient) {
    }

    getMap(mapId: number): Observable<Map> {
        return this.httpClient.get<MapResponse>(`/api/v2/maps/${mapId}`).pipe(
            map(response => Map.fromResponse(response))
        );
    }

    createMap(request: CreateMapRequest, image: File, progressCb: (progress: number) => void): Observable<Map> {
        const formData: FormData = new FormData();
        formData.append('request', JSON.stringify(request));
        formData.append('image', image);

        return this.httpClient.post<MapResponse>('/api/v2/maps/', formData, {reportProgress: true, observe: 'events'}).pipe(
            uploadProgress(progressCb),
            toResponseBody(),
            map(response => Map.fromResponse(response!))
        );
    }

    createMapLayer(mapId: number, request: CreateMapLayerRequest): Observable<MapLayer> {
        return this.httpClient.post<MapLayerResponse>(`/api/v2/maps/${mapId}/layers`, request).pipe(
            map(response => MapLayer.fromResponse(response))
        );
    }
}
