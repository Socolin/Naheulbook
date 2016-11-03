import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable, ReplaySubject} from 'rxjs/Rx';

import {Location, Map} from './location.model';
import {JsonService} from '../shared/json-service';
import {NotificationsService} from '../notifications';
import {LoginService} from "../user";

@Injectable()
export class LocationService extends JsonService {

    private locations: ReplaySubject<Location[]>;
    private locationsTree: ReplaySubject<Location[]>;

    constructor(http: Http
        , notification: NotificationsService
        , loginService: LoginService) {
        super(http, notification, loginService);
    }

    getLocationsTree(): Observable<Location[]> {
        if (!this.locationsTree) {
            this.locationsTree = new ReplaySubject<Location[]>(1);

            this._http.get('/api/location/list')
                .map(res => res.json())
                .subscribe(
                    (locations: Location[]) => {
                        let locationsById = {};
                        locations.forEach(l => {
                            l.sons = [];
                            locationsById[l.id] = l;
                        });
                        locations.forEach(l => {
                            if (l.parent) {
                                locationsById[l.parent].sons.push(l);
                            }
                        });

                        this.locationsTree.next(locations);
                    },
                    error => {
                        this.locationsTree.error(error);
                    }
                );
        }
        return this.locationsTree;
    }

    getLocations(): Observable<Location[]> {
        if (!this.locations) {
            this.locations = new ReplaySubject<Location[]>(1);

            this._http.get('/api/location/list')
                .map(res => res.json())
                .subscribe(
                    locations => {
                        this.locations.next(locations);
                        this.locations.complete();
                    },
                    error => {
                        this.locations.error(error);
                    }
                );
        }
        return this.locations;
    }

    getLocation(locationId: number): Observable<Location> {
        return this.postJson('/api/location/detail', {locationId: locationId})
            .map(res => res.json());
    }

    getMaps(locationId: number): Observable<Map[]> {
        return this.postJson('/api/location/maps', {locationId: locationId})
            .map(res => res.json());
    }

    editLocation(location: Location, maps: Map[]): Observable<any> {
        return this.postJson('/api/location/edit', {location: location, maps: maps})
            .map(res => res.json());
    }

    clearLocations() {
        this.locationsTree = null;
        this.locations = null;
    }

    addLocation(locationName: string, parentId: number): Observable<Location> {
        return this.postJson('/api/location/create', {
                parentId: parentId,
                locationName: locationName
            })
            .map(res => res.json());
    }

    listMapImages(filter: string): Observable<string[]> {
        return this.postJson('/api/location/listMapImages', {filter: filter})
            .map(res => res.json());
    }

    searchLocations(filter: string): Observable<Location[]> {
        return this.postJson('/api/location/search', {filter: filter})
            .map(res => res.json());
    }
}
