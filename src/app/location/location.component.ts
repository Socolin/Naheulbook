import {Component, Input} from '@angular/core';
import {Router} from '@angular/router';

import {Location, Map} from "./location.model";
import {LocationService} from './location.service';
import {TextFormatterPipe} from '../shared';

@Component({
    moduleId: module.id,
    selector: 'location',
    templateUrl: 'location.component.html',
    pipes: [TextFormatterPipe],
    directives: [LocationComponent]
})
export class LocationComponent {
    @Input() location: Location;
    @Input() editable: boolean;
    private folded: boolean = true;
    private maps: Map[];

    constructor(private _router: Router
        , private _locationService: LocationService) {
    }

    toggleFolded() {
        if (this.folded && !this.maps) {
            this._locationService.getMaps(this.location.id).subscribe(
                maps => {
                    this.maps = maps;
                }
            );
        }
        this.folded = !this.folded;
    }

    editLocation() {
        this._router.navigate(["/edit-location", this.location.id]);
    }
}
