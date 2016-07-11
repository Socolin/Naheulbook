import {Injectable} from '@angular/core';
import {Http} from '@angular/http';
import {Observable, ReplaySubject} from 'rxjs/Rx';

import {Location, Map} from './location.model';

@Injectable()
export class LocationService {

    private locations: ReplaySubject<Location[]>;
    private locationsTree: ReplaySubject<Location[]>;

    constructor(private _http: Http) {
    }

    getLocationsTree(): Observable<Location[]> {
        if (!this.locationsTree || this.locationsTree.isUnsubscribed) {
            this.locationsTree = new ReplaySubject<Location[]>(1);

            this._http.get('/api/location/list')
                .map(res => res.json())
                .subscribe(
                    locations => {
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
        if (!this.locations || this.locations.isUnsubscribed) {
            this.locations = new ReplaySubject<Location[]>(1);

            this._http.get('/api/location/list')
                .map(res => res.json())
                .subscribe(
                    locations => {
                        this.locations.next(locations);
                    },
                    error => {
                        this.locations.error(error);
                    }
                );
        }
        return this.locations;
    }

    getLocation(locationId: number): Observable<Location> {
        return this._http.post('/api/location/detail', JSON.stringify({locationId: locationId}))
            .map(res => res.json());
    }

    getMaps(locationId: number): Observable<Map[]> {
        return this._http.post('/api/location/maps', JSON.stringify({locationId: locationId}))
            .map(res => res.json());
    }

    editLocation(location: Location, maps: Map[]): Observable<any> {
        return this._http.post('/api/location/edit', JSON.stringify({location: location, maps: maps}))
            .map(res => res.json());
    }

    clearLocations() {
        this.locationsTree = null;
        this.locations = null;
    }

    addLocation(locationName: string, parentId: number): Observable<Location> {
        return this._http.post('/api/location/create', JSON.stringify({
                parentId: parentId,
                locationName: locationName
            }))
            .map(res => res.json());
    }

    listMapImages(filter: string): Observable<string[]> {
        return this._http.post('/api/location/listMapImages', JSON.stringify({filter: filter}))
            .map(res => res.json());
    }
}
