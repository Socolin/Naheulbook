import {Component} from '@angular/core';
import {AptitudeService} from './aptitude.service';
import {SummaryAptitudeGroupResponse} from '../api/responses/aptitude-response';
import {MatCard} from '@angular/material/card';
import {MatListItem, MatNavList} from '@angular/material/list';
import {RouterModule} from '@angular/router';

@Component({
    selector: 'app-aptitude-list',
    imports: [
        MatCard,
        MatNavList,
        MatListItem,
        RouterModule
    ],
    templateUrl: './aptitude-list.component.html',
    styleUrl: './aptitude-list.component.scss'
})
export class AptitudeListComponent {
    aptitudeGroups: SummaryAptitudeGroupResponse[] = [];

    constructor(
        private readonly aptitudeService: AptitudeService,
    ) {
        this.aptitudeService.getAptitudeGroups().subscribe((response) => {
            this.aptitudeGroups = response.aptitudeGroups
        })
    }
}
