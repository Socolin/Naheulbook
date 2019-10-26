import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {MapService} from './map.service';
import {MapSummaryResponse} from '../api/responses';

@Component({
    templateUrl: './map-list.component.html',
    styleUrls: ['./map-list.component.scss']
})
export class MapListComponent implements OnInit {
    public maps?: MapSummaryResponse[];

    constructor(
        private readonly router: Router,
        private readonly mapService: MapService,
    ) {
    }

    ngOnInit() {
        this.mapService.getMaps().subscribe(maps => {
            this.maps = maps;
        })
    }

    addMap() {
        this.router.navigate(['/map', 'create']);
    }
}
