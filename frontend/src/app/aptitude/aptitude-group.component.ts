import {Component, OnDestroy, OnInit} from '@angular/core';
import {AptitudeGroupResponse, AptitudeResponse} from '../api/responses/aptitude-response';
import {AptitudeService} from './aptitude.service';
import {ActivatedRoute} from '@angular/router';
import {Subscription} from 'rxjs';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {MatIconModule} from '@angular/material/icon';
import {MatCardModule} from '@angular/material/card';

@Component({
    selector: 'app-aptitude-group',
    imports: [
        MatProgressSpinnerModule,
        MatIconModule,
        MatCardModule
    ],
    templateUrl: './aptitude-group.component.html',
    styleUrl: './aptitude-group.component.scss'
})
export class AptitudeGroupComponent implements OnInit, OnDestroy {

    aptitudeGroup?: AptitudeGroupResponse;
    aptitudes: AptitudeResponse[] = [];
    private subscription = new Subscription();

    constructor(
        private readonly aptitudeService: AptitudeService,
        private readonly route: ActivatedRoute,
    ) {
    }

    ngOnInit() {
        this.subscription.add(this.route.params.subscribe(params => {
            let aptitudeGroupId = params['aptitudeGroupId'];
            this.aptitudeService.getAptitudeGroup(aptitudeGroupId).subscribe(aptitudeGroup => {
                this.aptitudeGroup = aptitudeGroup;
                this.aptitudes = aptitudeGroup.aptitudes.sort((a, b) => a.roll - b.roll);
            })
        }));
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }
}
