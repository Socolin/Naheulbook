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
    public newLocationName?: string;

    constructor(
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly notifications: NotificationsService,
        private readonly locationService: LocationService,
    ) {
    }

    addLocation() {
        if (!this.newLocationName) {
            throw new Error('addLocation: `newLocationName` should be defined');
        }

        this.locationService.addLocation(this.newLocationName, this.location.id).subscribe(
            location => {
                this.locationService.clearLocations();
                this.notifications.success('Cartographie', 'Lieu ajotuer');
                this.location.sons.push(location);
                this.newLocationName = undefined;
            }
        );
    }

    addLocationAndEdit() {
        if (!this.newLocationName) {
            throw new Error('addLocationAndEdit: `newLocationName` should be defined');
        }

        this.locationService.addLocation(this.newLocationName, this.location.id).subscribe(
            location => {
                this.locationService.clearLocations();
                this.notifications.success('Cartographie', 'Lieu ajotuer');
                this.router.navigate(['/edit-location', location.id]);
                this.newLocationName = undefined;
            }
        );
    }

    editLocation() {
        this.locationService.editLocation(this.location, this.maps).subscribe(
            () => {
                this.locationService.clearLocations();
                this.notifications.success('Cartographie', 'Lieu editer');
                this.router.navigate(['/database/locations']);
            }
        );
    }

    ngOnInit() {
        this.route.params.subscribe(
            params => {
                this.locationService.getLocation(+params['id']).subscribe(
                    location => {
                        if (!location.data) {
                            location.data = {};
                        }

                        this.location = location;
                    },
                    err => {
                        console.log(err);
                    });
                this.locationService.getMaps(+params['id']).subscribe(
                    maps => {
                        this.maps = maps;
                    },
                    err => {
                        console.log(err);
                    });
            });
    }
}
