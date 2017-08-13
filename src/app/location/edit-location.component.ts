import {Component, OnInit} from '@angular/core';
import {Router, ActivatedRoute} from '@angular/router';

import {Location, Map} from './location.model';
import {LocationService} from './location.service';
import {NotificationsService} from '../notifications';

@Component({
    templateUrl: './edit-location.component.html',
})
export class EditLocationComponent implements OnInit {
    public location: Location;
    public maps: Map[] = [];
    public newLocationName: string | undefined;

    constructor(private _route: ActivatedRoute
        , private _router: Router
        , private _notifications: NotificationsService
        , private _locationService: LocationService) {
    }

    addLocation() {
        if (!this.newLocationName) {
            throw new Error('addLocation: `newLocationName` should be defined');
        }

        this._locationService.addLocation(this.newLocationName, this.location.id).subscribe(
            location => {
                this._locationService.clearLocations();
                this._notifications.success('Cartographie', 'Lieu ajotuer');
                this.location.sons.push(location);
                this.newLocationName = undefined;
            }
        );
    }

    addLocationAndEdit() {
        if (!this.newLocationName) {
            throw new Error('addLocationAndEdit: `newLocationName` should be defined');
        }

        this._locationService.addLocation(this.newLocationName, this.location.id).subscribe(
            location => {
                this._locationService.clearLocations();
                this._notifications.success('Cartographie', 'Lieu ajotuer');
                this._router.navigate(['/edit-location', location.id]);
                this.newLocationName = undefined;
            }
        );
    }

    editLocation() {
        this._locationService.editLocation(this.location, this.maps).subscribe(
            () => {
                this._locationService.clearLocations();
                this._notifications.success('Cartographie', 'Lieu editer');
                this._router.navigate(['/database/locations']);
            }
        );
    }

    ngOnInit() {
        this._route.params.subscribe(
            params => {
                this._locationService.getLocation(+params['id']).subscribe(
                    location => {
                        if (!location.data) {
                            location.data = {};
                        }

                        this.location = location;
                    },
                    err => {
                        console.log(err);
                    });
                this._locationService.getMaps(+params['id']).subscribe(
                    maps => {
                        this.maps = maps;
                    },
                    err => {
                        console.log(err);
                    });
            });
    }
}
