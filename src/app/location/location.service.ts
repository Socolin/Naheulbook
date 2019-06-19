
import {map} from 'rxjs/operators';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, ReplaySubject} from 'rxjs';

import {Location, Map} from './location.model';

@Injectable()
export class LocationService {

    public locations: ReplaySubject<Location[]> | undefined;
    public locationsTree: ReplaySubject<Location[]> | undefined;

    constructor(private httpClient: HttpClient) {
    }

    getLocationsTree(): Observable<Location[]> {
        if (!this.locationsTree) {
            this.locationsTree = new ReplaySubject<Location[]>(1);

            this.httpClient.get<Location[]>('/api/location/list')
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

                        if (this.locationsTree) {
                            this.locationsTree.next(locations);
                        }
                    },
                    error => {
                        if (this.locationsTree) {
                            this.locationsTree.error(error);
                        }
                    }
                );
        }
        return this.locationsTree;
    }

    getLocations(): Observable<Location[]> {
        if (!this.locations) {
            this.locations = new ReplaySubject<Location[]>(1);

            this.httpClient.get<Location[]>('/api/location/list')
                .subscribe(
                    locations => {
                        if (this.locations) {
                            this.locations.next(locations);
                            this.locations.complete();
                        }
                    },
                    error => {
                        if (this.locations) {
                            this.locations.error(error);
                        }
                    }
                );
        }
        return this.locations;
    }

    getLocation(locationId: number): Observable<Location> {
        return this.httpClient.post<Location>('/api/location/detail', {locationId: locationId});
    }

    getMaps(locationId: number): Observable<Map[]> {
        return this.httpClient.post<Map[]>('/api/location/maps', {locationId: locationId});
    }

    editLocation(location: Location, maps: Map[]): Observable<any> {
        return this.httpClient.post<any>('/api/location/edit', {location: location, maps: maps});
    }

    clearLocations() {
        this.locationsTree = undefined;
        this.locations = undefined;
    }

    addLocation(locationName: string, parentId: number): Observable<Location> {
        return this.httpClient.post<Location>('/api/location/create', {
                parentId: parentId,
                locationName: locationName
            });
    }

    listMapImages(filter: string): Observable<string[]> {
        return this.httpClient.post<string[]>('/api/location/listMapImages', {filter: filter});
    }

    searchLocations(filter: string): Observable<Location[]> {
        return this.httpClient.post<Location[]>('/api/location/search', {filter: filter});
    }
}
