import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {ItemTemplate} from './item-template.model';
import {forkJoin} from 'rxjs';
import {OriginService} from '../origin';
import {God, MiscService} from '../shared';
import {JobService} from '../job';

export interface ItemDetailDialogData {
    itemTemplate: ItemTemplate;
}

@Component({
    selector: 'app-item-detail-dialog',
    templateUrl: './item-detail-dialog.component.html',
    styleUrls: ['./item-detail-dialog.component.scss']
})
export class ItemDetailDialogComponent implements OnInit {
    public originsName: { [originId: number]: string };
    public jobsName: { [jobId: number]: string };
    public godsByTechName: {[techName: string]: God};

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: ItemDetailDialogData,
        private _originService: OriginService,
        private _jobService: JobService,
        private _miscService: MiscService,
    ) {
    }

    ngOnInit() {
        forkJoin([
            this._jobService.getJobList(),
            this._originService.getOriginList(),
            this._miscService.getGodsByTechName(),
        ]).subscribe(([jobs, origins, godsByTechName]) => {
            this.godsByTechName = godsByTechName;
            this.jobsName = jobs.reduce((result, job) => {result[job.id] = job.name; return result;}, {});
            this.originsName = origins.reduce((result, origin) => {result[origin.id] = origin.name; return result;}, {});
        });
    }
}
