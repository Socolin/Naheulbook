import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {ItemTemplate} from './item-template.model';
import {forkJoin} from 'rxjs';
import {OriginService} from '../origin';
import {God, MiscService} from '../shared';
import {JobService} from '../job';

export interface ItemTemplateDialogData {
    itemTemplate: ItemTemplate;
}

@Component({
    templateUrl: './item-template-dialog.component.html',
    styleUrls: ['./item-template-dialog.component.scss']
})
export class ItemTemplateDialogComponent implements OnInit {
    public originsName: { [originId: number]: string };
    public jobsName: { [jobId: number]: string };
    public godsByTechName: { [techName: string]: God };

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: ItemTemplateDialogData,
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
            this.jobsName = jobs.reduce((result, job) => {
                result[job.id] = job.name;
                return result;
            }, {});
            this.originsName = origins.reduce((result, origin) => {
                result[origin.id] = origin.name;
                return result;
            }, {});
        });
    }
}
