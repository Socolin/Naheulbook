import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {MapLayerResponse, MapMarkerResponse, MapResponse} from '../api/responses';
import {CreateMapLayerRequest, MapMarkerRequest, CreateMapRequest} from '../api/requests';
import {map} from 'rxjs/operators';
import {Map, MapLayer, MapMarker, MapMarkerBase} from './map.model';

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

    deleteMapLayer(mapLayerId: number): Observable<void> {
        return this.httpClient.delete<void>(`/api/v2/mapLayers/${mapLayerId}`);
    }

    createMarker(mapLayer: MapLayer, request: MapMarkerRequest): Observable<MapMarker> {
        return this.httpClient.post<MapMarkerResponse>(`/api/v2/mapLayers/${mapLayer.id}/markers`, request).pipe(
            map(response => MapMarkerBase.fromResponse(mapLayer, response))
        );
    }

    editMarker(mapLayer: MapLayer, mapMarkerId: number, request: MapMarkerRequest): Observable<MapMarker> {
        return this.httpClient.put<MapMarkerResponse>(`/api/v2/mapMarkers/${mapMarkerId}`, request).pipe(
            map(response => MapMarkerBase.fromResponse(mapLayer, response))
        );
    }

    deleteMarker(mapMarkerId: number): Observable<void> {
        return this.httpClient.delete<void>(`/api/v2/mapMarkers/${mapMarkerId}`);
    }
}
