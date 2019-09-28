import {Component, Input} from '@angular/core';
import {Router} from '@angular/router';

import {Location, Map} from './location.model';
import {LocationService} from './location.service';

@Component({
    selector: 'location',
    templateUrl: './location.component.html',
})
export class LocationComponent {
    @Input() location: Location;
    @Input() editable: boolean;

    public folded = true;
    public maps: Map[];

    constructor(
        private readonly router: Router,
        private readonly locationService: LocationService,
    ) {
    }

    toggleFolded() {
        if (this.folded && !this.maps) {
            this.locationService.getMaps(this.location.id).subscribe(
                maps => {
                    this.maps = maps;
                }
            );
        }
        this.folded = !this.folded;
    }

    editLocation() {
        this.router.navigate(['/edit-location', this.location.id]);
    }
}
