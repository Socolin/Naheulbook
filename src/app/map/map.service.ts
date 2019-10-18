import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {MapResponse} from '../api/responses';
import {CreateMapRequest} from '../api/requests';
import {map} from 'rxjs/operators';
import {Map} from './map.model';

@Injectable()
export class MapService {
    constructor(private httpClient: HttpClient) {
    }

    getMap(mapId: number): Observable<Map> {
        return this.httpClient.get<MapResponse>(`/api/v2/maps/${mapId}`).pipe(
            map(response => Map.fromResponse(response))
        );
    }

    createMap(request: CreateMapRequest, image: File): Observable<MapResponse> {
        const formData: FormData = new FormData();
        formData.append('request', JSON.stringify(request));
        formData.append('image', image);

        return this.httpClient.post<MapResponse>('/api/v2/maps/', formData).pipe(
            map(response => Map.fromResponse(response))
        );
    }
}
