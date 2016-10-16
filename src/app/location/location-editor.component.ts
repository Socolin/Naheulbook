import {Component, Input, OnInit} from '@angular/core';

import {Location, Map} from "./location.model";
import {LocationService} from './location.service';
import {AutocompleteValue} from '../shared';
import {NotificationsService} from '../notifications';

@Component({
    selector: 'location-editor',
    templateUrl: 'location-editor.component.html'
})
export class LocationEditorComponent implements OnInit {
    @Input() location: Location;
    @Input() maps: Map[];
    private locations: Location[];
    private autocompleteFilesCallback: Function;
    private newMap: Map;

    constructor(private _notification: NotificationsService
        , private _locationService: LocationService) {
        this.newMap = new Map();
    }

    updateAutocomplete(filter: string) {
        return this._locationService.listMapImages(filter).map(list => list.map(e => new AutocompleteValue(e, e)));
    }

    addMap() {
        console.log(this.newMap);
        if (!this.newMap.name || !this.newMap.file) {
            this._notification.error('Erreur', 'Nom ou fichier manquant');
            return;
        }
        this.maps.push(this.newMap);
        this.newMap = new Map();
    }

    ngOnInit() {
        this.autocompleteFilesCallback = this.updateAutocomplete.bind(this);
        this._locationService.getLocations().subscribe(
            locations => {
                this.locations = locations;
            },
            err => {
                console.log(err);
            });
    }
}
