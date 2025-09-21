import {Component, OnDestroy, OnInit} from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import {MapService} from './map.service';
import {MapSummaryResponse} from '../api/responses';
import {GmModeService, removeDiacritics} from '../shared';
import {combineLatest, Subscription} from 'rxjs';
import { UntypedFormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import {startWith} from 'rxjs/operators';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatNavList, MatListItem } from '@angular/material/list';

@Component({
    templateUrl: './map-list.component.html',
    styleUrls: ['./map-list.component.scss'],
    imports: [MatButton, MatIcon, MatFormField, MatInput, FormsModule, ReactiveFormsModule, MatNavList, MatListItem, RouterLink]
})
export class MapListComponent implements OnInit, OnDestroy {
    public maps?: MapSummaryResponse[];
    public filterControl: UntypedFormControl = new UntypedFormControl('');
    private subscription: Subscription = new Subscription();

    constructor(
        private readonly router: Router,
        private readonly mapService: MapService,
        private readonly gmModeService: GmModeService
    ) {
    }

    ngOnInit() {
        this.subscription.add(combineLatest([
            this.gmModeService.gmMode,
            this.mapService.getMaps(),
            this.filterControl.valueChanges.pipe(startWith<string>(''))
        ]).subscribe(([gmMode, maps, filter]) => {
            this.maps = maps.filter(m => gmMode || ! m.data.isGm)
                .filter(m => !filter || removeDiacritics(m.name.toLowerCase()).indexOf(removeDiacritics(filter.toLowerCase())) !== -1)
                .sort((a, b) => a.name.localeCompare(b.name));
        }));
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }

    addMap() {
        this.router.navigate(['/map', 'create']);
    }
}
