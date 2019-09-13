import {Component, OnInit} from '@angular/core';
import {UsefulDataService} from '../useful-data.service';
import {CriticalData} from '../useful-data.model';

@Component({
    selector: 'app-epic-fails-dialog',
    templateUrl: './epic-fails-dialog.component.html',
    styleUrls: ['../../shared/full-screen-dialog.scss', './epic-fails-dialog.component.scss']
})
export class EpicFailsDialogComponent implements OnInit {
    public epicFails: { [name: string]: CriticalData[] };

    constructor(
        public usefulDataService: UsefulDataService
    ) {
    }

    ngOnInit() {
        this.epicFails = this.usefulDataService.getEpifailData();
    }

}
