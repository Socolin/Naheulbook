import {Component, OnInit} from '@angular/core';
import {NotificationsService} from '../notifications';

import {Location} from './location.model';
import {LocationService} from './location.service';
import {LoginService} from '../user';

@Component({
    selector: 'location-list',
    templateUrl: './location-list.component.html'
})
export class LocationListComponent implements OnInit {
    private locations: Location[];
    private rootLocation: Location;
    private editable: boolean;

    constructor(private _locationService: LocationService
        , private _loginService: LoginService
        , private _notification: NotificationsService) {
    }

    getLocations() {
        this._locationService.getLocationsTree().subscribe(
            locations => {
                this.locations = locations;
                this.locations.forEach(l => {
                    if (!l.parent) {
                        this.rootLocation = l;
                    }
                });
            },
            err => {
                this._notification.error('Erreur', 'Erreur serveur');
                console.log(err);
            }
        );
    }

    ngOnInit() {
        this.getLocations();
        this._loginService.loggedUser.subscribe(
            user => {
                this.editable = user && user.admin;
            },
            err => {
                this.editable = false;
                console.log(err);
            });
    }
}
